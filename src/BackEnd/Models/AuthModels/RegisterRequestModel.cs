using System.ComponentModel.DataAnnotations;

namespace TsuShopWebApi.Models;

public class RegisterRequestModel
{
    [Required]
    [MinLength(6)]
    public string Username { get; set; }
    [Required]
    [MinLength(6)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one letter, one number, and one special character.")]
    public string Password { get; set; }
}