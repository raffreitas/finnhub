using FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Repositories;
using FinnHub.MarketData.WebApi.Features.Assets.Infrastructure.Repositories;

namespace FinnHub.MarketData.WebApi.Features.Assets;

public static class DependencyInjection
{
    public static void AddAssetsFeature(this IServiceCollection services)
    {
        services.AddScoped<IAssetRepository, AssetRepository>();

        services.AddScoped<CreateAssetHandler>();
        services.AddScoped<CreateAssetValidator>();
    }
}
