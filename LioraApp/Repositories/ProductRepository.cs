using LioraApp.Data;
using LioraApp.Models;
using LioraApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LioraApp.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetWithDetailsAsync(int id)
        => await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        => await _context.Products
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .Include(p => p.Images.Where(i => i.IsMain))
            .Include(p => p.Category)
            .ToListAsync();
}
