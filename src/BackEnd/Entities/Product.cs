using System.Text.Json.Serialization;

namespace TsuShopWebApi.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public double Price { get; set; }
    public double Rating { get; set; }
    public int Quantity { get; set; }
    public ICollection<Review> Reviews { get; set; }
    [JsonIgnore]
    public ICollection<CartItem> CartItems { get; set; }
}