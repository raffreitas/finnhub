using FinnHub.MarketData.WebApi.Features.Assets;

namespace FinnHub.MarketData.WebApi.Setup;

public static class FeaturesConfiguration
{
    public static void AddFeaturesConfiguration(this IServiceCollection services)
    {
        services.AddAssetsFeature();
    }
}
