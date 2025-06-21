using FinnHub.PortfolioManagement.Infrastructure.Messaging.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Messaging.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddMessagingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetAndConfigureSettings<MessagingSettings>(configuration, MessagingSettings.SectionName);
        return services;
    }
}
