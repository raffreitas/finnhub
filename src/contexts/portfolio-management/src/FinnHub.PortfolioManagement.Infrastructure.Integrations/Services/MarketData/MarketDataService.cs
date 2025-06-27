using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Application.Abstractions.Tokens;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Models;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Settings;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Shared.Policies;
using FinnHub.Shared.Core;

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

    public async Task<Result<decimal>> GetCurrentMarketValueAsync(string assetSymbol, CancellationToken cancellationToken)
    {
        var result = await PolicyFactory
            .CreateDefaultPolicy<GetCurrentMarketValueResponse>()
            .ExecuteAsync(async () =>
            {
                var accessToken = await tokenService.GetAccessToken();
                return await $"{_settings.BaseUrl}"
                   .AppendPathSegment("api/v1/quotes")
                   .AppendPathSegment(assetSymbol)
                   .AppendPathSegment("current-price")
                   .WithOAuthBearerToken(accessToken)
                   .GetJsonAsync<GetCurrentMarketValueResponse>(cancellationToken: cancellationToken);
            });

        return result.IsFailure
            ? Result.Failure<decimal>(result.Error)
            : Result.Success(result.Value.CurrentPrice);
    }
}
