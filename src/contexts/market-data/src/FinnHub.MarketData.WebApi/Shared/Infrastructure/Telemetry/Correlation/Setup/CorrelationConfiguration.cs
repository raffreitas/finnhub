using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Context;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Factory;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.HttpClient;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Setup;
internal static class CorrelationConfiguration
{
    public static IServiceCollection AddCorrelationConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<ICorrelationContextAccessor, CorrelationContextAccessor>();
        services.AddSingleton<ICorrelationContextFactory, CorrelationContextFactory>();

        services.TryAddTransient<CorrelationIdDelegateHandler>();

        services.ConfigureAll<HttpClientFactoryOptions>(options => options
            .HttpMessageHandlerBuilderActions.Add(builder => builder
                .AdditionalHandlers.Add(builder.Services.GetRequiredService<CorrelationIdDelegateHandler>()
                )
            )
        );

        return services;
    }
}
