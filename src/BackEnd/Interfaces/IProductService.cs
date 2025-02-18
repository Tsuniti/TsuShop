using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface IProductService
{
    Task<ProductsPage> GetSomeAsync(int page, int quantity, int minPrice, int maxPrice, string? sortBy, bool isAscending, string? category, string? name);
    Task<double> GetMaxPrice();

    Task<ICollection<string>> GetCategories();

    Task<Product?> GetByIdAsync(Guid productId);

    Task<Product?> CreateAsync(string name, string description, string category, double price, int quantity, Guid userId);

    Task<Product?> UpdateAsync(Guid productId, string name, string description, string category, double price, int quantity, Guid userId);

    Task<bool> DeleteAsync(Guid productId, Guid userId);
    Task<bool> RemoveSomeFromStockAsync(Guid productId, int quantity);
    Task<bool> AddSomeToStockAsync(Guid productId, int quantity, Guid userId);
    
    Task<bool> ReCountRatingAsync(Product product);
}