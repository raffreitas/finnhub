using System.Security.Claims;

using FinnHub.PortfolioManagement.Application.Abstractions.Users;

using Microsoft.AspNetCore.Http;

namespace FinnHub.PortfolioManagement.Infrastructure.Authentication.Services;

internal sealed class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId => GetUserId();

    private Guid GetUserId()
    {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userId) ? throw new UnauthorizedAccessException() : Guid.Parse(userId);
    }
}
