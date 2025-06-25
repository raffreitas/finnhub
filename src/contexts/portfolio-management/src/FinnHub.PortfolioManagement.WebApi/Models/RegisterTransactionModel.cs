using FinnHub.PortfolioManagement.Application.Commands.RegisterBuyAsset;
using FinnHub.PortfolioManagement.Application.Commands.RegisterSellAsset;

namespace FinnHub.PortfolioManagement.WebApi.Models;

public sealed record RegisterTransactionModel
{
    public required string AssetSymbol { get; set; }
    public required int Quantity { get; set; }
    public required decimal PricePerUnit { get; set; }
    public required DateTimeOffset TransactionDate { get; set; }

    public RegisterBuyAssetRequest ToBuyAssetRequest(Guid portfolioId)
    {
        return new RegisterBuyAssetRequest
        {
            PortfolioId = portfolioId,
            AssetSymbol = AssetSymbol,
            Quantity = Quantity,
            PricePerUnit = PricePerUnit,
            TransactionDate = TransactionDate
        };
    }

    public RegisterSellAssetRequest ToSellAssetRequest(Guid portfolioId)
    {
        return new RegisterSellAssetRequest
        {
            PortfolioId = portfolioId,
            AssetSymbol = AssetSymbol,
            Quantity = Quantity,
            PricePerUnit = PricePerUnit,
            TransactionDate = TransactionDate
        };
    }
}
