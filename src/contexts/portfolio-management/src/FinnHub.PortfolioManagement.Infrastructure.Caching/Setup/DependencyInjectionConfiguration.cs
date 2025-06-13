using FinnHub.PortfolioManagement.Infrastructure.Caching.Settings;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Caching.Setup;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddCachingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = GetSettings(services, configuration);

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(settings.DefaultExpirationInMinutes),
                Expiration = TimeSpan.FromMinutes(settings.DefaultExpirationInMinutes)
            };
        });
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
            options.InstanceName = settings.CacheName;
        });
        return services;
    }

    private static CacheSettings GetSettings(IServiceCollection services, IConfiguration configuration, string section = CacheSettings.SectionName)
    {
        services.AddOptions<CacheSettings>()
            .BindConfiguration(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var cacheSettings = configuration.GetSection(section).Get<CacheSettings>()
            ?? throw new ArgumentException($"{nameof(CacheSettings)} should be configured.");

        return cacheSettings;

    }
}
