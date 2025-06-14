using System.Security;

using FinnHub.PortfolioManagement.Application.Abstractions.Users;

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FinnHub.PortfolioManagement.Infrastructure.Authentication.Services;

internal sealed class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId => GetUserId();

    private Guid GetUserId()
    {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return string.IsNullOrEmpty(userId) ? throw new SecurityException() : Guid.Parse(userId);
    }
}
