using System.Text.Json.Serialization;

namespace TsuShopWebApi.Entities;

public class CartItem : BaseEntity
{
    public int Quantity { get; set; }
    
    public Guid ProductId { get; set; }
    [JsonIgnore]
    public Product Product { get; set; }
    
    public Guid CartId { get; set; }
    [JsonIgnore]
    public Cart Cart { get; set; }
}