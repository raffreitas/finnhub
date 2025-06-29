using FinnHub.MarketData.Shared.Infrastructure.Authentication.Setup;
using FinnHub.MarketData.Shared.Infrastructure.Database.Setup;
using FinnHub.MarketData.Shared.Infrastructure.Messaging.Setup;
using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Setup;
using FinnHub.MarketData.WebApi.Features;
using FinnHub.MarketData.WebApi.Shared.Presentation.Setup;

namespace FinnHub.MarketData.WebApi;

public static class StartupHelper
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        services.AddTelemetryConfiguration(configuration);
        services.AddAuthenticationConfiguration(configuration);
        services.AddDatabaseConfiguration(configuration);
        services.AddMessagingConfiguration(configuration);
        services.AddHealthCheckConfiguration();
        services.AddEndpointsConfiguration();
        services.AddOpenApiConfiguration();
        services.AddFeaturesConfiguration(configuration);
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseTelemetryConfiguration();
        app.UseAuthenticationConfiguration();
        app.UseHealthCheckConfiguration();
        app.UseEndpointsConfiguration();
        app.UseOpenApiConfiguration();
        app.UseHttpsRedirection();
    }
}
