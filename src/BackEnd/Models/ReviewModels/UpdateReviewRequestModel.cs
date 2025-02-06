using System.ComponentModel.DataAnnotations;

namespace TsuShopWebApi.Models.ReviewModels;

public class UpdateReviewRequestModel
{
    [Required]
    public Guid ReviewId { get; set; }
    [Required]
    public int Rating { get; set; }
    public string? Text { get; set; }
    
}