using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Middleware;
using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Setup;
using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Middleware;
using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FinnHub.MarketData.Shared.Infrastructure.Telemetry.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddTelemetryConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetAndConfigureSettings<TelemetrySettings>(
            configuration,
            TelemetrySettings.SectionName
        );

        if (settings.IsEnabled)
            services.ConfigureOpenTelemetry(settings);


        services.AddCorrelationConfiguration();

        return services;
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
                }))
            .WithLogging(builder => builder
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(cfg =>
                {
                    cfg.Endpoint = new Uri(openTelemetryEndpoint);
                    cfg.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }),
                options =>
                {
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                    options.IncludeFormattedMessage = true;
                });
    }
}
