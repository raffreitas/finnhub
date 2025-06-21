using FinnHub.PortfolioManagement.Infrastructure.Caching.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Caching.Setup;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddCachingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetAndConfigureSettings<CacheSettings>(configuration, CacheSettings.SectionName);

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
}
