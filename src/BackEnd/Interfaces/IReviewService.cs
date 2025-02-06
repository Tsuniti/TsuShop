using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface IReviewService
{
    Task<ICollection<Review>?> GetAllByProductIdAsync(Guid productId);
    Task<Review?> CreateAsync(Guid productId, string? text, int rating, Guid userId);
    Task<Review?> UpdateAsync(Guid reviewId, string? text, int rating, Guid userId);
    Task<bool> RemoveAsync(Guid reviewId, Guid userId);
    
}