using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Setup;

internal static class MarketDataConfiguration
{
    public static IServiceCollection AddMarketDataConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.GetAndConfigureSettings<MarketDataSettings>(configuration, MarketDataSettings.SectionName);
        services.AddScoped<IMarketDataService, MarketDataService>();
        return services;
    }
}
