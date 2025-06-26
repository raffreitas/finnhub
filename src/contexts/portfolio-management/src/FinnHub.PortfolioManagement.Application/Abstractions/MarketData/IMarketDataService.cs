namespace FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
public interface IMarketDataService
{
    Task<decimal> GetCurrentMarketValueAsync(string assetSymbol, CancellationToken cancellationToken);
}
