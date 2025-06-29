using FinnHub.MarketData.Ingestion.Workers.Binance;
using FinnHub.MarketData.Ingestion.Workers.Binance.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

namespace FinnHub.MarketData.Ingestion.Setup;
internal static class HostedServiceConfiguration
{
    public static IServiceCollection AddHostedServicesConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.GetAndConfigureSettings<BinanceSettings>(configuration, BinanceSettings.SectionName);
        services.AddHostedService<BinanceIngestionService>();
        return services;
    }
}
