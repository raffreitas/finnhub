using FinnHub.PortfolioManagement.Application.Abstractions.Messaging;
using FinnHub.PortfolioManagement.Infrastructure.Messaging.Services.RabbitMQ;
using FinnHub.PortfolioManagement.Infrastructure.Messaging.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

namespace FinnHub.PortfolioManagement.Infrastructure.Messaging.Setup;

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
