using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Middleware;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Setup;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Middleware;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Setup;
public static class DependencyInjectionConfiguration
{
    public static WebApplicationBuilder AddTelemetryConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var settings = services.GetAndConfigureSettings<TelemetrySettings>(
            configuration,
            TelemetrySettings.SectionName
        );

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
                .AddSource("FinnHub.MarketData.Messaging")
                .AddSource("FinnHub.MarketData.Binance")
                .AddSource("FinnHub.MarketData.Sync")
                .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource()
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

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: settings.ServiceName,
                serviceVersion: settings.ServiceVersion
            );

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.IncludeFormattedMessage = true;
            options.SetResourceBuilder(resourceBuilder);
            options.AddOtlpExporter(option =>
            {
                option.Endpoint = new Uri(endpoint);
                option.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
        });
    }
}
