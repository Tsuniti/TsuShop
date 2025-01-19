using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}