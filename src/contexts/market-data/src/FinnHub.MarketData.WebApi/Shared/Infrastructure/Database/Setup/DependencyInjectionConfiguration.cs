using FinnHub.MarketData.WebApi.Features.Assets.Domain.Enums;
using FinnHub.MarketData.WebApi.Shared.Extensions;
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
        var settings = services.GetAndConfigureSettings<DatabaseSettings>(configuration, DatabaseSettings.SectionName);

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

        return services;
    }
}
