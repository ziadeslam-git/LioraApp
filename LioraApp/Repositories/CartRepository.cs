using LioraApp.Data;
using LioraApp.Models;
using LioraApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LioraApp.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Cart?> GetCartByUserIdAsync(string userId)
        => await _context.Carts
            .AsSplitQuery()
            .Include(c => c.Items)
                .ThenInclude(ci => ci.GiftBundle)
            .Include(c => c.Items)
                .ThenInclude(ci => ci.ProductVariant!)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Images.Where(i => i.IsMain))
            .FirstOrDefaultAsync(c => c.UserId == userId);
}
