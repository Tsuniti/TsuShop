using System.ComponentModel.DataAnnotations;

namespace TsuShopWebApi.Models;

public class LoginRequestModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}