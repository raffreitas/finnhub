using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Services;

using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Authentication.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContextService>();
        return services;
    }
}
