using FinnHub.MarketData.WebApi.Features.Quotes.Commands.SaveMarketData;
using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Repositories;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance.Settings;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Repositories;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Sync;
using FinnHub.MarketData.WebApi.Shared.Extensions;

namespace FinnHub.MarketData.WebApi.Features.Quotes;

public static class DependencyInjection
{
    public static IServiceCollection AddQuotesFeature(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.GetAndConfigureSettings<BinanceSettings>(configuration, BinanceSettings.SectionName);

        services.AddHostedService<BinanceDataIngestionService>();
        services.AddHostedService<DataIngestionSyncService>();

        services.AddScoped<IQuoteRepository, QuoteRepository>();

        services.AddScoped<SaveMarketDataHandler>();

        return services;
    }
}
