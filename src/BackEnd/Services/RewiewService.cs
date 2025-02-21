using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;

namespace TsuShopWebApi.Services;

public class ReviewService : IReviewService
{
    private readonly IApplicationDbContext _context;
    private readonly IProductService _productService;

    public ReviewService(IApplicationDbContext context, IProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<ICollection<Review>?> GetAllByProductIdAsync(Guid productId)
    {
        return await _context.Reviews
            .Where(review => review.ProductId == productId)
            .Include(review => review.User)
            .OrderByDescending(review => review.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Review?> CreateAsync(Guid productId, string? text, int rating, Guid userId)
    {
        if (!await _context.Users.AnyAsync(user => user.Id == userId))
        {
            return null; // user not exists
        }

        var product = await _context.Products
            .Include(product => product.Reviews)
            .FirstOrDefaultAsync(product => product.Id == productId);

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
            return review;
        }

        return null; // if not added
    }

    public async Task<Review?> UpdateAsync(Guid reviewId, string? text, int rating, Guid userId)
    {

        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null)
        {
            return null; // user not exists
        }

        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review is null)
        {
            return null;
        }

        if (review.UserId != userId && !user.IsAdmin)
        {
            return null;
        }

        review.Text = text;
        review.Rating = rating;
        review.UpdatedAt = DateTime.UtcNow;

        var product =
            await _context.Products
                .Include(product => product.Reviews)
                .FirstOrDefaultAsync(product => product.Id == review.ProductId);

        Console.WriteLine(product);
        
        if (await _context.SaveChangesAsync() > 0)
        {
            Console.WriteLine(await _productService.ReCountRatingAsync(product));
            
            return review;
        }

        return null;
    }

    public async Task<bool> RemoveAsync(Guid reviewId, Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null)
        {
            return false; // user not exists
        }

        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review is null)
        {
            return false;
        }
        
        if (review.UserId != userId && !user.IsAdmin)
        {
            return false;
        }

        _context.Reviews.Remove(review);

        var product = await _context.Products
            .Include(product => product.Reviews)
            .FirstOrDefaultAsync(product => product.Id == review.ProductId);
        
        if (await _context.SaveChangesAsync() > 0)
        {
            await _productService.ReCountRatingAsync(product);
            return true;
        }

        return false;
    }
}