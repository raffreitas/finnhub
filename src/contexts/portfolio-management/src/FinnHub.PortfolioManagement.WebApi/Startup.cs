using FinnHub.PortfolioManagement.Application.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Setup;
using FinnHub.PortfolioManagement.WebApi.Setup;

namespace FinnHub.PortfolioManagement.WebApi;

public static class StartupHelper
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        builder.AddTelemetryConfiguration();
        services.AddPresentationConfiguration();
        services.AddOpenApiConfiguration();
        services.AddPersistenceConfiguration(configuration);
        services.AddAuthenticationConfiguration(configuration);
        services.AddApplicationConfiguration();
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseTelemetryConfiguration();
        app.UseOpenApiConfiguration();
        app.UseHttpsRedirection();
        app.UseAuthenticationConfiguration();
        app.UsePresentationConfiguration();
    }
}
