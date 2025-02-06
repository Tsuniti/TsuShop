using System.ComponentModel.DataAnnotations;

namespace TsuShopWebApi.Models.ReviewModels;

public class CreateReviewRequestModel
{
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public int Rating { get; set; }
    public string? Text { get; set; }
}