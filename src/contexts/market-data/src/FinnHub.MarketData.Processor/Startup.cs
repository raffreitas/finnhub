using FinnHub.MarketData.Processor.Setup;
using FinnHub.MarketData.Shared.Infrastructure.Database.Setup;
using FinnHub.MarketData.Shared.Infrastructure.Messaging.Setup;
using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Setup;
using FinnHub.MarketData.WebApi.Features;

namespace FinnHub.MarketData.Processor;
internal static class Startup
{
    public static void ConfigureHost(HostApplicationBuilder host)
    {
        var services = host.Services;
        var configuration = host.Configuration;

        services.AddHostedServicesConfiguration(configuration);
        services.AddMessagingConfiguration(configuration);
        services.AddTelemetryConfiguration(configuration);
        services.AddDatabaseConfiguration(configuration);
        services.AddFeaturesConfiguration(configuration);
    }
}
