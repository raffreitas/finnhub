using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Application.Abstractions.Tokens;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Models;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Settings;

using Flurl;
using Flurl.Http;

using Microsoft.Extensions.Options;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData;

internal sealed class MarketDataService(
    IOptions<MarketDataSettings> options,
    ITokenService tokenService
) : IMarketDataService
{
    private readonly MarketDataSettings _settings = options.Value;

    public async Task<decimal> GetCurrentMarketValueAsync(string assetSymbol, CancellationToken cancellationToken)
    {
        // TODO: Add resilience policies.
        // TODO: Add caching to avoid unnecessary API calls.
        // TODO: Add authentication
        var accessToken = await tokenService.GetAccessToken();

        var result = await $"{_settings.BaseUrl}"
            .AppendPathSegment("api/v1/quotes")
            .AppendPathSegment(assetSymbol)
            .AppendPathSegment("current-price")
            .WithOAuthBearerToken(accessToken)
            .GetJsonAsync<GetCurrentMarketValueResponse>(cancellationToken: cancellationToken);

        return result.CurrentPrice;
    }

}
