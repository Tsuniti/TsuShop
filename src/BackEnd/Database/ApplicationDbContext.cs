using System.Security.Cryptography;
using System.Text;
using FirstTodoWebApi.Options;
using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;

namespace TsuShopWebApi.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly DatabaseOptions _databaseOptions;
    private readonly FirstUserOptions _firstUserOptions;
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }
    

    public ApplicationDbContext(DatabaseOptions databaseOptions, FirstUserOptions firstUserOptions)
    {
        _databaseOptions = databaseOptions;
        _firstUserOptions = firstUserOptions;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_databaseOptions.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(user => user.Cart)
            .WithOne(cart => cart.User)
            .HasForeignKey<Cart>(cart => cart.UserId);

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

        modelBuilder.Entity<User>()
            .HasData(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = _firstUserOptions.Username,
                    PasswordHash =
                        BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(_firstUserOptions.Password))),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
    }
}