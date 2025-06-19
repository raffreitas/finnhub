using FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

namespace FinnHub.MarketData.WebApi.Features.Assets;

public static class DependencyInjection
{
    public static void AddAssetsFeature(this IServiceCollection services)
    {
        services.AddScoped<CreateAssetHandler>();
        services.AddScoped<CreateAssetValidator>();
    }
}
