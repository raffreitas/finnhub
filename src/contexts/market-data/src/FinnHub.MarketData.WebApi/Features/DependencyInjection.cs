using FinnHub.MarketData.WebApi.Features.Assets;
using FinnHub.MarketData.WebApi.Features.Quotes;

namespace FinnHub.MarketData.WebApi.Features;

public static class DependencyInjection
{
    public static void AddFeaturesConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAssetsFeature();
        services.AddQuotesFeature(configuration);
    }
}
