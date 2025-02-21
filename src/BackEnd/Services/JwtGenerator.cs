using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Options;

namespace TsuShopWebApi.Services;

public class JwtGenerator : IJwtGenerator
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUserService _userService;

    public TokenValidationParameters TokenValidationParameters { get; }

    public JwtGenerator(IOptions<JwtOptions>  jwtOptions, IUserService userService)
    {

        _jwtOptions = jwtOptions.Value;
        _userService = userService;

        TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience
            
        };
    }

    public async Task<string> GenerateTokenAsync(Guid userId)
    {
        var isUserAdmin = await _userService.IsUserAdminAsync(userId);
        
        var claims = new List<Claim>
        {
            new Claim("IsAdmin", isUserAdmin.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        
        // из секретного ключа формирируем "подпись" токена - его шифрование
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TtlMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}