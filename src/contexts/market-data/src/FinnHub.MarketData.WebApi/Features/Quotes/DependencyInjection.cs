using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance.Settings;
using FinnHub.MarketData.WebApi.Shared.Extensions;

namespace FinnHub.MarketData.WebApi.Features.Quotes;

public static class DependencyInjection
{
    public static IServiceCollection AddQuotesFeature(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.GetAndConfigureSettings<BinanceSettings>(configuration, BinanceSettings.SectionName);
        services.AddHostedService<BinanceDataIngestionService>();

        return services;
    }
}
