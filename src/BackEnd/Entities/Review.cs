using System.Text.Json.Serialization;

namespace TsuShopWebApi.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; }
    public string Text { get; set; }
    
    public Guid UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    
    public Guid ProductId { get; set; }
    [JsonIgnore]
    public Product Product { get; set; }
}