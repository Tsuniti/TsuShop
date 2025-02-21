using Microsoft.IdentityModel.Tokens;

namespace TsuShopWebApi.Interfaces;

public interface IJwtGenerator
{
    
    TokenValidationParameters TokenValidationParameters { get; }
    Task<string> GenerateTokenAsync(Guid userId);
}