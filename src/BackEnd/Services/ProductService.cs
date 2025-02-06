using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;

namespace TsuShopWebApi.Services;

public class ProductService : IProductService
{
    
    private readonly IApplicationDbContext _context;
    private readonly IUserService _userService;

    public ProductService(IApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }
    
    
    public async Task<ICollection<Product>> GetSomeAsync(int page = 1, int quantity = 21, string? sortBy = null, bool isAscending = true, string? category = null, string? name = null)
    {

        var productsQuery = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            productsQuery = productsQuery.Where(product => product.Category == category);
        }

        if (!string.IsNullOrEmpty(name))
        {
            productsQuery = productsQuery.Where(product => product.Name.Contains(name));
        }
        
        if (!string.IsNullOrEmpty(sortBy))
        {
            if (isAscending is false) // если не указано или false, то по убыванию, иначе по возрастанию
            {
                // Сортировка по убыванию
                productsQuery = productsQuery.OrderByDescending(product => EF.Property<object>(product, sortBy));

            }
            else
            {
                // Сортировка по возрастанию
                productsQuery = productsQuery.OrderBy(product => EF.Property<object>(product, sortBy));
            }
        }
        else
        {
            // По умолчанию сортировка по имени
            productsQuery = productsQuery.OrderBy(product => product.Name);
        }

        // Ограничение по количеству
        var products = await productsQuery
                                                    .Skip(quantity * (page - 1))
                                                    .Take(quantity)
                                                    .ToListAsync();

        return products;
        
    }

    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        return await _context.Products
            .Include(product => product.Reviews)
            .FirstOrDefaultAsync(product => product.Id == productId);
    }

    public async Task<Product?> CreateAsync(string name, string description, string category, double price, int quantity, Guid userId)
    {
        
        Console.WriteLine("Creating");

        if (!await _userService.IsUserAdminAsync(userId)) 
            return null;


        var newProduct = new Product
        {

            Name = name,
            Description = description,
            Category = category,
            Price = price,
            Rating = 0,
            Quantity = quantity,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
            
        };
        Console.WriteLine(newProduct);
        await _context.Products.AddAsync(newProduct);
        
        return await _context.SaveChangesAsync() > 0 ? newProduct : null;
    }

    public async Task<Product?> UpdateAsync(Guid productId, string name, string description, string category, double price, int quantity,
        Guid userId)
    {
        var product = await GetByIdAsync(productId);
        if (product is null || !await _userService.IsUserAdminAsync(userId))
            return null;

        product.Name = name;
        product.Description = description;
        product.Category = category;
        product.Price = price;
        product.Quantity = quantity;

        product.UpdatedAt = DateTime.UtcNow;
        
        
        return await _context.SaveChangesAsync() > 0 ? product : null;
    }

    public async Task<bool> DeleteAsync(Guid productId, Guid userId)
    {
        if (!await _userService.IsUserAdminAsync(userId)) 
            return false;
        
        var product = await GetByIdAsync(productId);

        if (product is null)
            return false;

        _context.Products.Remove(product);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveSomeFromStockAsync(Guid productId, int quantity)
    {
        var product = await GetByIdAsync(productId);

        if (product is null)
            return false;

        product.Quantity -= quantity;
        product.UpdatedAt = DateTime.UtcNow;
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddSomeToStockAsync(Guid productId, int quantity, Guid userId)
    {
        if (!await _userService.IsUserAdminAsync(userId)) 
            return false;
        
        var product = await GetByIdAsync(productId);

        if (product is null)
            return false;

        product.Quantity += quantity;
        product.UpdatedAt = DateTime.UtcNow;
        
        return await _context.SaveChangesAsync() > 0;
    }
    

    public async Task<bool> ReCountRatingAsync(Product product)
    {
        double sum = 0;

        foreach (var r in product.Reviews)
        {
            sum += r.Rating;
        }
        product.Rating = sum / product.Reviews.Count;

        if (await _context.SaveChangesAsync() > 0)
        {
            return true;
        }

        return false;
    }
}