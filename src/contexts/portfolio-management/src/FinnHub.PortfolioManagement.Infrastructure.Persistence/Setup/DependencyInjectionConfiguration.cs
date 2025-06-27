using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.Queries;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Queries;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Repositories;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Settings;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Shared;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddPersistenceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetAndConfigureSettings<DatabaseSettings>(configuration, DatabaseSettings.SectionName);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(settings.ConnectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(settings.CommandTimeoutInSeconds);
                sqlOptions.EnableRetryOnFailure();
            }).UseSnakeCaseNamingConvention());

        services.AddHealthChecksConfiguration(settings);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IPortfolioQueries, PortfolioQueries>();

        return services;
    }

    private static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, DatabaseSettings settings)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(settings.ConnectionString);
        return services;
    }
}
