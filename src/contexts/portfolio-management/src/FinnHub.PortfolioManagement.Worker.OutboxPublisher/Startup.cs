using FinnHub.PortfolioManagement.Infrastructure.Messaging.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Setup;
using FinnHub.PortfolioManagement.Worker.OutboxPublisher.Setup;

namespace FinnHub.PortfolioManagement.Worker.OutboxPublisher;

internal static class Startup
{
    public static void ConfigureHost(HostApplicationBuilder host)
    {
        var services = host.Services;
        var configuration = host.Configuration;

        services.AddPersistenceConfiguration(configuration);
        services.AddMessagingConfiguration(configuration);
        services.AddHostedServicesConfiguration(configuration);
        services.AddTelemetryConfiguration(configuration);
    }
}
