namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Models;

internal sealed record class GetCurrentMarketValueResponse
{
    public decimal CurrentPrice { get; init; }
}
