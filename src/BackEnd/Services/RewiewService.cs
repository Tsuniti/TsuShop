using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;

namespace TsuShopWebApi.Services;

public class ReviewService : IReviewService
{

    private readonly IApplicationDbContext _context;
    private readonly IProductService _productService;

    ReviewService(IApplicationDbContext context, IProductService productService)
    {
        _context = context;
        _productService = productService;

    }
    
    public async Task<ICollection<Review>?> GetAllByProductIdAsync(Guid productId)
    {
        return await _context.Reviews
            .Where(review => review.ProductId == productId)
            .ToListAsync();
    }

    public async Task<Review?> CreateAsync(Guid productId, Guid userId, string? text, int rating)
    {
        if (!await _context.Users.AnyAsync(user => user.Id == userId))
        {
            return null; // user not exists
        }

        var product = await _productService.GetByIdAsync(productId);

        if (product is null)
        {
            return null; // product not exists
        }

        if (product.Reviews.Any(review => review.UserId == userId))
        {
            return null; // review on this product from user already exist
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),

            Rating = rating,
            Text = text ?? string.Empty,
            UserId = userId,
            ProductId = productId,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow

        };

        await _context.Reviews.AddAsync(review);
        
        
        if (await _context.SaveChangesAsync() > 0)
        {
            await _productService.ReCountRatingAsync(product);
        }

        return null; // if not added
    }

    public async Task<Review?> UpdateAsync(Guid productId, Guid userId, string? text, int rating)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveAsync(Guid productId, Guid userId)
    {
        throw new NotImplementedException();
    }
    
}