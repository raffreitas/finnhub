using FinnHub.Shared.Core;

namespace FinnHub.PortfolioManagement.Application.Abstractions.MarketData;

public interface IMarketDataService
{
    Task<Result<decimal>> GetCurrentMarketValueAsync(string assetSymbol, CancellationToken cancellationToken);
}
