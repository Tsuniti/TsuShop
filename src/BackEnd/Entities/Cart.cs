using System.Text.Json.Serialization;

namespace TsuShopWebApi.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    [JsonIgnore]
    public ICollection<CartItem>? CartItems { get; set; }
}