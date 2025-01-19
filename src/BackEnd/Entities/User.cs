using System.Text.Json.Serialization;

namespace TsuShopWebApi.Entities;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public Guid CartId { get; set; }
    [JsonIgnore]
    public Cart Cart { get; set; }
    [JsonIgnore]
    public ICollection<Review> Reviews { get; set; }
}