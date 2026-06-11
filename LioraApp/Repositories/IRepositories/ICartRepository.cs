using LioraApp.Models;

namespace LioraApp.Repositories.IRepositories;

public interface ICartRepository : IRepository<Cart>
{
    /// <summary>Gets a user's cart with all items and their product variant details.</summary>
    Task<Cart?> GetCartByUserIdAsync(string userId);
}
