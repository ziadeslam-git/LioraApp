using LioraApp.Data;
using LioraApp.Models;
using LioraApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LioraApp.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        => await _context.Orders
            .AsSplitQuery()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Images.Where(i => i.IsMain))
            .Include(o => o.Address)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId);

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId)
        => await _context.Orders
            .Where(o => o.UserId == userId)
            .AsSplitQuery()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Images.Where(i => i.IsMain))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersByUserPagedAsync(
        string userId, int page = 1, int pageSize = 10)
    {
        var query = _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var orders = await query
            .AsSplitQuery()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Images.Where(i => i.IsMain))
            .ToListAsync();

        return (orders, totalCount);
    }
}
