using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Settings;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Shared.MessageHandlers;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

using Polly;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Setup;

internal static class MarketDataConfiguration
{
    public static IServiceCollection AddMarketDataConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetAndConfigureSettings<MarketDataSettings>(configuration, MarketDataSettings.SectionName);

        services
            .AddHttpClient<IMarketDataService, MarketDataService>(client => client.BaseAddress = new Uri(settings.BaseUrl))
            .AddHttpMessageHandler<AccessTokenHttpMessageHandler>()
            .AddResilienceHandler(MarketDataService.ResilienceHandlerName, pipelineBuilder => pipelineBuilder
                .AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 5,
                    Delay = TimeSpan.FromMilliseconds(500),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                }));
        return services;
    }
}
