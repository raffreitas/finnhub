using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Common.Authentication;

internal sealed class JwtTokenBuilder
{
    private const string TEST_JWT_SECRET_KEY = "22ef12f3-a580-4393-9b44-8f1a776578ab-22ef12f3-a580-4393-9b44-8f1a776578ab";
    private Guid UserId { get; set; } = Guid.NewGuid();

    public JwtTokenBuilder WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }

    public string Build()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TEST_JWT_SECRET_KEY));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
