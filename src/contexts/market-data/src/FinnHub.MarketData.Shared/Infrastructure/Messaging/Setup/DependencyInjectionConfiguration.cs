using FinnHub.MarketData.Shared.Infrastructure.Messaging.Services;
using FinnHub.MarketData.Shared.Infrastructure.Messaging.Services.RabbitMQ;
using FinnHub.MarketData.Shared.Infrastructure.Messaging.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

namespace FinnHub.MarketData.Shared.Infrastructure.Messaging.Setup;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddMessagingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetAndConfigureSettings<MessagingSettings>(
            configuration,
            MessagingSettings.SectionName
        );

        services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
        {
            Uri = new Uri(settings.ConnectionString)
        });

        services.AddSingleton<RabbitMQConnectionManager>();
        services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

        return services;
    }
}
