using Microsoft.IdentityModel.Tokens;

namespace TsuShopWebApi.Interfaces;

public interface IJwtGenerator
{
    
    TokenValidationParameters TokenValidationParameters { get; }
    string GenerateToken(Guid userId);
}