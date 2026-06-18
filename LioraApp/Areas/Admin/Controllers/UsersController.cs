using System.Net;
using LioraApp.Data;
using LioraApp.Models;
using LioraApp.Repositories.IRepositories;
using LioraApp.Utilities;
using LioraApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LioraApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UsersController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private const int PageSize = 10;

    public UsersController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _unitOfWork  = unitOfWork;
        _userManager = userManager;
        _context     = context;
    }

    // GET: /Admin/Users — DB-level pagination: filters + Skip/Take pushed to SQL before materialization
    public async Task<IActionResult> Index(int page = 1, string? searchQuery = null, string? statusFilter = null)
    {
        // 1. Build IQueryable with all filters applied at DB level
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchQuery))
            query = query.Where(u => u.FullName.Contains(searchQuery) || (u.Email != null && u.Email.Contains(searchQuery)));

        if (statusFilter == "Active")
            query = query.Where(u => u.IsActive);
        else if (statusFilter == "Inactive")
            query = query.Where(u => !u.IsActive);

        // 2. Count at DB level (no data transferred)
        int totalCount = await query.CountAsync();
        int activeCount = await query.CountAsync(u => u.IsActive);
        int inactiveCount = totalCount - activeCount;
        int joinedRecentlyCount = await query.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        if (page > totalPages)
            page = totalPages;

        // 3. Fetch only the current page's users from DB
        var pagedUsers = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        // Fix 11: Single JOIN query replaces N GetRolesAsync calls (was N DB round-trips,
        // now 1 query for all users on this page).
        var pageUserIds = pagedUsers.Select(u => u.Id).ToList();

        var userRoles = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => pageUserIds.Contains(ur.UserId))
            .Join(_context.Roles,
                ur => ur.RoleId,
                r  => r.Id,
                (ur, r) => new { ur.UserId, RoleName = r.Name! })
            .ToListAsync();

        var rolesByUser = userRoles
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => (IList<string>)g.Select(x => x.RoleName).ToList());

        // 4. Batch-load orders only for the current page's user IDs
        var pageUserIdSet = pageUserIds.ToHashSet();
        var pageOrders = await _unitOfWork.Orders
            .FindAllAsync(o => pageUserIdSet.Contains(o.UserId), tracked: false);
        var ordersByUser = pageOrders
            .GroupBy(o => o.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 5. Build VMs only for this page's users (max PageSize iterations)
        var vms = new List<UserAdminVM>();
        foreach (var user in pagedUsers)
        {
            var roles      = rolesByUser.TryGetValue(user.Id, out var rl) ? rl : new List<string>();
            var userOrders = ordersByUser.TryGetValue(user.Id, out var ord) ? ord : new();

            vms.Add(new UserAdminVM
            {
                Id            = user.Id,
                FullName      = user.FullName,
                Email         = user.Email ?? string.Empty,
                IsActive      = user.IsActive,
                CreatedAt     = user.CreatedAt,
                Roles         = roles,
                TotalOrders   = userOrders.Count,
                TotalSpent    = userOrders.Sum(o => o.TotalAmount),
                LastOrderDate = userOrders.OrderByDescending(o => o.CreatedAt).FirstOrDefault()?.CreatedAt
            });
        }

        ViewBag.CurrentPage   = page;
        ViewBag.TotalPages    = totalPages;
        ViewBag.TotalCount    = totalCount;
        ViewBag.ActiveCount   = activeCount;
        ViewBag.InactiveCount = inactiveCount;
        ViewBag.JoinedRecentlyCount = joinedRecentlyCount;
        ViewBag.PageSize      = PageSize;
        ViewBag.SearchQuery   = searchQuery;
        ViewBag.StatusFilter  = statusFilter;
        ViewData["Title"]     = "Users";
        return View(vms);
    }

    // GET: /Admin/Users/Details/{id} — BUG #2: populate stats + recent orders
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles  = await _userManager.GetRolesAsync(user);
        var orders = await _unitOfWork.Orders
            .FindAllAsync(o => o.UserId == user.Id, tracked: false);
        var orderList = orders.ToList();

        var vm = new UserAdminVM
        {
            Id            = user.Id,
            FullName      = user.FullName,
            Email         = user.Email ?? string.Empty,
            IsActive      = user.IsActive,
            CreatedAt     = user.CreatedAt,
            Roles         = roles,
            TotalOrders   = orderList.Count,
            TotalSpent    = orderList.Sum(o => o.TotalAmount),
            LastOrderDate = orderList.OrderByDescending(o => o.CreatedAt).FirstOrDefault()?.CreatedAt,
            RecentOrders  = orderList
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new OrderSummaryForUserVM
                {
                    Id            = o.Id,
                    TotalAmount   = o.TotalAmount,
                    Status        = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    CreatedAt     = o.CreatedAt
                }).ToList()
        };

        ViewData["Title"] = "User Details";
        return View(vm);
    }

    // POST: /Admin/Users/ToggleActive/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // NEVER deactivate yourself (logged-in Admin)
        var currentUserId = _userManager.GetUserId(User);
        if (user.Id == currentUserId)
        {
            TempData["error"] = "You cannot deactivate your own account.";
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        // Invalidate existing cookie session immediately when deactivating
        if (!user.IsActive)
            await _userManager.UpdateSecurityStampAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportWord()
    {
        var users = await _userManager.Users.AsNoTracking().OrderByDescending(u => u.CreatedAt).ToListAsync();
        
        var sb = new System.Text.StringBuilder();
        sb.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>");
        sb.Append("<head><meta charset='utf-8'><title>Customers Export</title>");
        sb.Append("<style>");
        sb.Append("body { font-family: 'Inter', sans-serif; color: #191c1e; }");
        sb.Append("h1 { color: #4648d4; text-align: center; font-family: 'Inter', sans-serif; margin-bottom: 20px; }");
        sb.Append("table { width: 100%; border-collapse: collapse; margin-top: 20px; border: 1px solid #e0e3e5; }");
        sb.Append("th { background-color: #f2f4f6; color: #464554; padding: 12px; text-align: left; border: 1px solid #e0e3e5; font-size: 14px; text-transform: uppercase; }");
        sb.Append("td { padding: 12px; border: 1px solid #e0e3e5; color: #191c1e; font-size: 14px; }");
        sb.Append(".active { color: #059669; font-weight: bold; background-color: #d1fae5; padding: 4px 8px; border-radius: 4px; }");
        sb.Append(".inactive { color: #ba1a1a; font-weight: bold; background-color: #fee2e2; padding: 4px 8px; border-radius: 4px; }");
        sb.Append("</style>");
        sb.Append("</head><body>");
        sb.Append("<h1>Customers Report</h1>");
        sb.Append("<table>");
        sb.Append("<tr><th>Customer Name</th><th>Email Address</th><th>Status</th><th>Joined Date</th></tr>");
        
        foreach (var u in users)
        {
            // Fix 3: HtmlEncode every user-controlled value before embedding in HTML
            // to prevent XSS when the exported .doc file is opened.
            var safeName   = WebUtility.HtmlEncode(u.FullName ?? string.Empty);
            var safeEmail  = WebUtility.HtmlEncode(u.Email   ?? string.Empty);
            var status     = u.IsActive
                ? "<span class='active'>Active</span>"
                : "<span class='inactive'>Inactive</span>";
            sb.Append($"<tr><td>{safeName}</td><td>{safeEmail}</td><td>{status}</td><td>{u.CreatedAt:MMM dd, yyyy}</td></tr>");
        }
        
        sb.Append("</table></body></html>");
        
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "application/msword", "Customers_Export.doc");
    }
}
