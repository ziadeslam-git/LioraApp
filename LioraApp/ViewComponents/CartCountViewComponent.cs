using System.Security.Claims;
using System.Threading.Tasks;
using LioraApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LioraApp.ViewComponents;

public class CartCountViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _db;

    public CartCountViewComponent(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return View(0);

        var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return View(0);

        var count = await _db.CartItems
            .Where(ci => ci.CartId == cart.Id)
            .SumAsync(ci => (int?)ci.Quantity) ?? 0;

        return View(count);
    }
}
