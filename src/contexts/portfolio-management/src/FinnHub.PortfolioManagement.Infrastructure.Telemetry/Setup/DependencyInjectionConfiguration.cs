using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Correlation.Middleware;
using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Correlation.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Middleware;
using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Npgsql;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FinnHub.PortfolioManagement.Infrastructure.Telemetry.Setup;
public static class DependencyInjectionConfiguration
{
    public static WebApplicationBuilder AddTelemetryConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var settings = GetSettings(services, configuration);

        if (settings.IsEnabled)
        {
            builder.ConfigureLogging(settings);
            services.ConfigureOpenTelemetry(settings);
        }

        services.AddCorrelationConfiguration();

        return builder;
    }

    public static WebApplication UseTelemetryConfiguration(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }

    private static void ConfigureOpenTelemetry(
        this IServiceCollection services,
        TelemetrySettings settings
    )
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: settings.ServiceName,
                serviceVersion: settings.ServiceVersion
            );

        var openTelemetryEndpoint = settings.OpenTelemetryEndpoint
            ?? throw new ArgumentException($"{nameof(TelemetrySettings.OpenTelemetryEndpoint)} should be configured.");

        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddSource(settings.ServiceName)
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddNpgsql()
                .AddEntityFrameworkCoreInstrumentation(cfg => cfg.SetDbStatementForText = true)
                .AddOtlpExporter(cfg =>
                {
                    cfg.Endpoint = new Uri(openTelemetryEndpoint);
                    cfg.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }))
            .WithLogging(builder => builder
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(cfg =>
                {
                    cfg.Endpoint = new Uri(openTelemetryEndpoint);
                    cfg.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }))
            .WithMetrics(builder => builder
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(cfg =>
                {
                    cfg.Endpoint = new Uri(openTelemetryEndpoint);
                    cfg.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }));
    }

    private static void ConfigureLogging(
        this WebApplicationBuilder builder,
        TelemetrySettings settings
    )
    {
        var endpoint = settings.OpenTelemetryEndpoint
            ?? throw new ArgumentException($"{nameof(TelemetrySettings.OpenTelemetryEndpoint)} should be configured.");

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.IncludeFormattedMessage = true;
            options.AddOtlpExporter(option =>
            {
                option.Endpoint = new Uri(endpoint);
                option.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
        });
    }

    private static TelemetrySettings GetSettings(
        IServiceCollection services,
        ConfigurationManager configuration,
        string section = TelemetrySettings.SectionName
    )
    {
        services.AddOptions<TelemetrySettings>()
            .BindConfiguration(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = configuration.GetSection(section).Get<TelemetrySettings>()
            ?? throw new ArgumentException($"{nameof(TelemetrySettings)} should be configured.");

        return settings;
    }
}
