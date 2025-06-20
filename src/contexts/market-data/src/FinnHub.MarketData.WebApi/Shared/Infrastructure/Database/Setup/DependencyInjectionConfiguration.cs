using FinnHub.MarketData.WebApi.Features.Assets.Domain.Repositories;
using FinnHub.MarketData.WebApi.Shared.Domain.Enums;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Database.Settings;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Database.Setup;

internal static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = GetSettings(services, configuration);

        services.AddSingleton<IMongoClient>(sp =>
        {
            var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);

            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());

            return new MongoClient(clientSettings);
        });

        services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new EnumSerializer<AssetType>(BsonType.String));

        services.AddScoped<IAssetRepository, AssetRepository>();

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
