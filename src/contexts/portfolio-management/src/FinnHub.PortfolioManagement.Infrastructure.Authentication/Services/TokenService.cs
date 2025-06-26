using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FinnHub.PortfolioManagement.Application.Abstractions.Tokens;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Settings;
using FinnHub.PortfolioManagement.Infrastructure.Caching.Abstractions;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinnHub.PortfolioManagement.Infrastructure.Authentication.Services;

internal class TokenService(
    IUserContext userContext,
    IOptions<AuthenticationSettings> options,
    ICacheService cacheService
) : ITokenService
{
    private readonly AuthenticationSettings _settings = options.Value;
    public async Task<string> GetAccessToken()
    {
        var cacheKey = $"AccessToken:{userContext.UserId}";

        var accessToken = await cacheService.GetOrCreateAsync(
            key: cacheKey,
            factory: _ => ValueTask.FromResult(GenerateToken(userContext.UserId)),
            expiration: TimeSpan.FromHours(_settings.JwtExpiration) - TimeSpan.FromMinutes(5)
        );

        return accessToken;
    }

    private string GenerateToken(Guid userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.JwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_settings.JwtExpiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
