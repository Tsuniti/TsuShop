using System.ComponentModel.DataAnnotations;

namespace TsuShopWebApi.Models.CartModels;

public class AddSomeInCartRequestModel
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
    public int Quantity { get; set; }
}