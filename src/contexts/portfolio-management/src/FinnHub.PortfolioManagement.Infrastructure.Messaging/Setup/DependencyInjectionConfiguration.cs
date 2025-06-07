using FinnHub.PortfolioManagement.Infrastructure.Messaging.Settings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Messaging.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddMessagingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = GetSettings(services, configuration);
        return services;
    }

    private static MessagingSettings GetSettings(
        IServiceCollection services,
        IConfiguration configuration,
        string section = MessagingSettings.SectionName)
    {
        // TODO: Add data annotations validation for CacheSettings
        services.AddOptions<MessagingSettings>()
            .BindConfiguration(section)
            .ValidateOnStart();

        var settings = configuration.GetSection(section).Get<MessagingSettings>()
            ?? throw new ArgumentException($"{nameof(MessagingSettings)} should be configured.");

        return settings;

    }
}
