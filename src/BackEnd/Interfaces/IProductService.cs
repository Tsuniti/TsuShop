using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface IProductService
{
    Task<ICollection<Product>> GetSomeAsync(int page, int quantity, string? sortBy, bool isAscending, string? category, string? name);

    Task<Product?> GetByIdAsync(Guid productId);

    Task<Product?> CreateAsync(string name, string description, string category, double price, int quantity, Guid userId);

    Task<Product?> UpdateAsync(Guid productId, string name, string description, string category, double price, int quantity, Guid userId);

    Task<bool> DeleteAsync(Guid productId, Guid userId);
    Task<bool> RemoveSomeFromStockAsync(Guid productId, int quantity);
    Task<bool> AddSomeToStockAsync(Guid productId, int quantity, Guid userId);

    Task<bool> IsAvailableAsync(Guid productId, int quantity);
    Task<bool> ReCountRatingAsync(Product product);
}