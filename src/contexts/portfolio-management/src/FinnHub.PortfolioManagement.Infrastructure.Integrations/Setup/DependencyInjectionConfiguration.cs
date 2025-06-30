using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Shared.MessageHandlers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddIntegrationsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AccessTokenHttpMessageHandler>();

        services.AddMarketDataConfiguration(configuration);

        return services;
    }
}
