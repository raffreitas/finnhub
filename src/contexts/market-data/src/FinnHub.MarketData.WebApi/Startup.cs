using FinnHub.MarketData.WebApi.Setup;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Database.Setup;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Setup;

namespace FinnHub.MarketData.WebApi;

public static class StartupHelper
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.AddTelemetryConfiguration();

        builder.Services.AddCorsConfiguration();
        builder.Services.AddAuthenticationConfiguration(builder.Configuration);
        builder.Services.AddDatabaseConfiguration(builder.Configuration);
        builder.Services.AddHealthCheckConfiguration();
        builder.Services.AddEndpointsConfiguration();
        builder.Services.AddOpenApiConfiguration();
        builder.Services.AddFeaturesConfiguration();
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseCorsConfiguration();
        app.UseTelemetryConfiguration();
        app.UseAuthenticationConfiguration();
        app.UseHealthCheckConfiguration();
        app.UseEndpointsConfiguration();
        app.UseOpenApiConfiguration();
        app.UseHttpsRedirection();
    }
}
