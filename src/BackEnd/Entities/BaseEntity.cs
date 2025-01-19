using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TsuShopWebApi.Entities;


public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}