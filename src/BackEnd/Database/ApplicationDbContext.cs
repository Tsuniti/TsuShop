using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Options;

namespace TsuShopWebApi.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly DatabaseOptions _databaseOptions;
    private readonly AdminOptions _adminOptions;
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }
    

    public ApplicationDbContext(IOptions<DatabaseOptions> databaseOptions, IOptions<AdminOptions> adminOptions)
    {
        _databaseOptions = databaseOptions.Value;
        _adminOptions = adminOptions.Value;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_databaseOptions.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>()
            .HasOne(cart => cart.User)
            .WithOne(user => user.Cart)
            .HasForeignKey<User>(user => user.CartId);

        modelBuilder.Entity<User>()
            .HasMany(user => user.Reviews)
            .WithOne(review => review.User)
            .HasForeignKey(review => review.UserId);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique()
            .HasFilter(null);

        modelBuilder.Entity<Review>()
            .HasOne(review => review.Product)
            .WithMany(product => product.Reviews)
            .HasForeignKey(review => review.ProductId);
        
        modelBuilder.Entity<Cart>()
            .HasMany(cart => cart.CartItems)
            .WithOne(cartItem => cartItem.Cart)
            .HasForeignKey(cartItem => cartItem.CartId);
        
        modelBuilder.Entity<Product>()
            .HasMany(product => product.CartItems)
            .WithOne(cartItem => cartItem.Product)
            .HasForeignKey(cartItem=> cartItem.ProductId);
        
        modelBuilder.Entity<CartItem>()
            .HasIndex(cartItem => new { cartItem.CartId, cartItem.ProductId })
            .IsUnique();
        
        
        modelBuilder.Entity<Cart>()
            .HasData(
                new Cart
                {
                    Id = Guid.Parse(_adminOptions.CartId),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
        
        modelBuilder.Entity<User>()
            .HasData(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = _adminOptions.Username,
                    PasswordHash =
                        BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(_adminOptions.Password))),
                    IsAdmin = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CartId = Guid.Parse(_adminOptions.CartId)
                    
                });
    }
}