using System.Net.Http.Json;

using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Models;
using FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Settings;
using FinnHub.Shared.Core;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData;

internal sealed class MarketDataService : IMarketDataService
{
    public const string ResilienceHandlerName = $"{nameof(MarketDataService)}Resilience";

    private readonly HttpClient _httpClient;
    private readonly ILogger<MarketDataService> _logger;

    public MarketDataService(
        ILogger<MarketDataService> logger,
        IOptions<MarketDataSettings> options,
        HttpClient httpClient
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
    }

    public async Task<Result<decimal>> GetCurrentMarketValueAsync(string assetSymbol, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<GetCurrentMarketValueResponse>(
                $"api/v1/quotes/{assetSymbol}/current-price",
                cancellationToken
            );

            return response is null
                ? Result.Failure<decimal>(Error.Problem("MarketData.Integration", "There was an error retrieving market data"))
                : Result.Success(response.CurrentPrice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on retrieving market data");
            return Result.Failure<decimal>(Error.Problem("MarketData.Integration", "There was an error retrieving market data"));
        }
    }
}
