using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface IReviewService
{
    Task<ICollection<Review>?> GetAllByProductIdAsync(Guid productId);
    Task<Review?> CreateAsync(Guid productId, Guid userId, string? text, int rating);
    Task<Review?> UpdateAsync(Guid productId, Guid userId, string? text, int rating);
    Task<bool> RemoveAsync(Guid productId, Guid userId);
    
}