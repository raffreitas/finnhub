using FinnHub.PortfolioManagement.Infrastructure.Logger.Logging.Setup;

using Microsoft.AspNetCore.Builder;

namespace FinnHub.PortfolioManagement.Infrastructure.Logger.Setup;
public static class DependencyInjectionConfiguration
{
    public static WebApplicationBuilder AddTelemetryConfiguration(this WebApplicationBuilder builder)
    {
        builder.AddLoggingConfiguration();
        return builder;
    }

    public static WebApplication UseTelemetryConfiguration(this WebApplication app)
    {
        app.UseLoggingConfiguration();
        return app;
    }
}
