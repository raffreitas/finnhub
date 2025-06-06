using FinnHub.PortfolioManagement.Infrastructure.Logger.Logging.Middlewares;

using Microsoft.AspNetCore.Builder;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace FinnHub.PortfolioManagement.Infrastructure.Logger.Logging.Setup;
internal static class DependencyInjectionConfiguration
{
    public static WebApplicationBuilder AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console());

        return builder;
    }

    public static WebApplication UseLoggingConfiguration(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<LogScopedMiddleware>();
        return app;
    }
}
