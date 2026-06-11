using LioraApp.Models;

namespace LioraApp.Repositories.IRepositories;

public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>Gets all top-level categories (no parent) with their children.</summary>
    Task<IEnumerable<Category>> GetTopLevelWithChildrenAsync();
}
