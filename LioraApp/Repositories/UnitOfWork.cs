using LioraApp.Data;
using LioraApp.Models;
using LioraApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LioraApp.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    // ─── Specific Repositories ───
    public IProductRepository Products { get; private set; }
    public IOrderRepository Orders { get; private set; }
    public ICartRepository Carts { get; private set; }
    public ICategoryRepository Categories { get; private set; }
    public IRepository<GiftBundle> GiftBundles { get; private set; }
    public IRepository<GiftBundleProduct> GiftBundleProducts { get; private set; }

    // ─── Generic Repositories ───
    public IRepository<ProductVariant> ProductVariants { get; private set; }
    public IRepository<ProductImage> ProductImages { get; private set; }
    public IRepository<CartItem> CartItems { get; private set; }
    public IRepository<Address> Addresses { get; private set; }
    public IRepository<OrderItem> OrderItems { get; private set; }
    public IRepository<Payment> Payments { get; private set; }
    public IRepository<Discount> Discounts { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Products       = new ProductRepository(_context);
        Orders         = new OrderRepository(_context);
        Carts          = new CartRepository(_context);
        Categories     = new CategoryRepository(_context);
        GiftBundles     = new Repository<GiftBundle>(_context);
        GiftBundleProducts = new Repository<GiftBundleProduct>(_context);
        ProductVariants = new Repository<ProductVariant>(_context);
        ProductImages   = new Repository<ProductImage>(_context);
        CartItems       = new Repository<CartItem>(_context);
        Addresses       = new Repository<Address>(_context);
        OrderItems      = new Repository<OrderItem>(_context);
        Payments        = new Repository<Payment>(_context);
        Discounts       = new Repository<Discount>(_context);
    }

    public async Task<int> SaveAsync()
        => await _context.SaveChangesAsync();

    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await _context.Database.BeginTransactionAsync();

    public void SetRowVersion<T>(T entity, byte[] rowVersion) where T : class
        => _context.Entry(entity).Property("RowVersion").OriginalValue = rowVersion;

    public void Dispose()
        => _context.Dispose();
}
