using System.ComponentModel.DataAnnotations;

namespace TsuShopWebApi.Models.ReviewModels;

public class RemoveReviewRequestModel
{
    [Required]
    public Guid ReviewId { get; set; }
}