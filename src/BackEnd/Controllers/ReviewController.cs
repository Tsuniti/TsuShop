using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Models.ReviewModels;

namespace TsuShopWebApi.Controllers;

[Route("review")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Get all reviews by product id
    /// </summary>
    /// <param name="productId">Guid</param>
    /// <returns>list of reviews</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] Guid productId)
    {
        var reviews = await _reviewService.GetAllByProductIdAsync(productId);

        return Ok(reviews);
    }

    /// <summary>
    /// Create review
    /// </summary>
    /// <param name="model">Model with (Guid)ProductId, (string)Text, (int)Rating</param>
    /// <returns>review</returns>
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestModel model)
    {
        var review = await _reviewService.CreateAsync(model.ProductId, model.Text, model.Rating, UserId);
        if (review is null)
            return BadRequest("Review is not created");

        return Ok(review);
    }
    
    /// <summary>
    /// Update review
    /// </summary>
    /// <param name="model">Model with (Guid)ProductId, (string)Text, (int)Rating</param>
    /// <returns>review</returns>
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut]
    public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewRequestModel model)
    {
        var review = await _reviewService.UpdateAsync(model.ReviewId, model.Text, model.Rating, UserId);
        
        if (review is null)
            return BadRequest("Review is not updated");

        return Ok(review);
    }
    
    /// <summary>
    /// Remove review
    /// </summary>
    /// <param name="model">Model with (Guid)ProductId</param>
    /// <returns>string</returns>
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public async Task<IActionResult> RemoveReview([FromBody] RemoveReviewRequestModel model)
    {
        if (!await _reviewService.RemoveAsync(model.ReviewId, UserId))
            return BadRequest("Review is not removed");

        return Ok("Review removed");
    }
}