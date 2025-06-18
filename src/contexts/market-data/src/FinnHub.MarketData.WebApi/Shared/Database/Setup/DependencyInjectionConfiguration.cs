using FinnHub.MarketData.WebApi.Shared.Database.Context;
using FinnHub.MarketData.WebApi.Shared.Database.Settings;

using Microsoft.EntityFrameworkCore;

namespace FinnHub.MarketData.WebApi.Shared.Database.Setup;

internal static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = GetSettings(services, configuration);

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseMongoDB(settings.ConnectionString, settings.DatabaseName)
            .UseSnakeCaseNamingConvention()
        );

        return services;
    }

    private static DatabaseSettings GetSettings(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .BindConfiguration(DatabaseSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = configuration
            .GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>()
                ?? throw new ArgumentException($"{nameof(DatabaseSettings)} should be configured.");

        return settings;
    }
}
