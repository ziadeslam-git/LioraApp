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
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork  = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Fix 7: All aggregations pushed to SQL — no in-memory loading of full rows.

        // ── Scalar counts (each = one fast SQL COUNT) ────────────────────────
        var totalProducts = await _unitOfWork.Products.Query()
            .AsNoTracking()
            .CountAsync(p => p.IsActive);

        var totalOrders = await _unitOfWork.Orders.Query()
            .AsNoTracking()
            .CountAsync();

        var pendingOrders = await _unitOfWork.Orders.Query()
            .AsNoTracking()
            .CountAsync(o => o.Status == SD.Status_Pending);

        var customers = await _userManager.Users
            .AsNoTracking()
            .CountAsync(u => u.IsActive);

        // ── Revenue: SUM only on Paid payments ───────────────────────────────
        var revenue = await _unitOfWork.Payments.Query()
            .AsNoTracking()
            .Where(p => p.Status == SD.Payment_Paid)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        // ── Monthly revenue for the current year (12-element array) ──────────
        var currentYear = DateTime.UtcNow.Year;
        var monthlyRevenue = new decimal[12];

        var paidThisYear = await _unitOfWork.Payments.Query()
            .AsNoTracking()
            .Where(p => p.Status == SD.Payment_Paid && p.CreatedAt.Year == currentYear)
            .Select(p => new { p.CreatedAt.Month, p.Amount })
            .ToListAsync();

        foreach (var p in paidThisYear)
            monthlyRevenue[p.Month - 1] += p.Amount;

        // ── Recent orders: server-side Take(5), projected to ViewModel ───────
        var recentOrders = await _unitOfWork.Orders.Query()
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Address)
            .OrderByDescending(o => o.Id)
            .Take(5)
            .Select(o => new RecentOrderVM
            {
                Id           = o.Id,
                CustomerName = o.User != null
                    ? o.User.FullName
                    : (o.Address != null ? o.Address.FullName : "N/A"),
                CreatedAt   = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status      = o.Status
            })
            .ToListAsync();

        // ── Top products: single DB-level GROUP BY + ORDER BY + Take(5) ──────
        var topProducts = await _unitOfWork.OrderItems.Query()
            .AsNoTracking()
            .Where(oi => oi.Order.Status != SD.Status_Cancelled)
            .GroupBy(oi => oi.ProductName)
            .Select(g => new TopProductVM
            {
                ProductName  = g.Key,
                CategoryName = "—",   // join Product.Category if needed in future
                TotalSold    = g.Sum(x => x.Quantity),
                ImageUrl     = ""
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToListAsync();

        // ── Fix 14: Real 7-day order count ───────────────────────────────────
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        var dailyCounts = await _unitOfWork.Orders.Query()
            .AsNoTracking()
            .Where(o => o.CreatedAt >= sevenDaysAgo)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        var weeklyData = new int[7];
        var today = DateTime.UtcNow.Date;
        foreach (var d in dailyCounts)
        {
            var offset = (today - d.Date).Days;
            if (offset >= 0 && offset < 7)
                weeklyData[6 - offset] = d.Count;
        }

        var maxVal = weeklyData.Max();
        
        // ── Fix 15: Dynamic Greeting ─────────────────────────────────────────
        var hour = DateTime.Now.Hour; // Local server time
        var greeting = hour switch
        {
            < 12 => "Good morning",
            < 17 => "Good afternoon",
            _    => "Good evening"
        };
        var displayName = User.Identity?.Name ?? "Admin";

        ViewBag.TotalProducts       = totalProducts;
        ViewBag.TotalOrders         = totalOrders;
        ViewBag.PendingOrders       = pendingOrders;
        ViewBag.PendingOrdersForNav = pendingOrders;
        ViewBag.TotalRevenue        = revenue;
        ViewBag.TotalCustomers      = customers;
        ViewBag.MonthlyRevenue      = monthlyRevenue;
        ViewBag.RecentOrders        = recentOrders;
        ViewBag.TopProducts         = topProducts;
        ViewBag.WeeklyOrders        = weeklyData;
        ViewBag.WeeklyOrdersMax     = maxVal == 0 ? 1 : maxVal;
        ViewBag.Greeting            = greeting;
        ViewBag.AdminDisplayName    = displayName;

        return View();
    }
}
