using LioraApp.Data;
using LioraApp.Models;
using LioraApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LioraApp.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Category>> GetTopLevelWithChildrenAsync()
        => await _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.SubCategories)
            .OrderBy(c => c.Name)
            .ToListAsync();
}
