using FinnHub.Shared.Core;

namespace FinnHub.MarketData.WebApi.Features.Assets.Errors;

public static class AssetErrors
{
    public static Error AssetSymbolNotUnique(string symbol) => Error.Conflict(
        "Asset.SymbolNotUnique",
        $"Asset with symbol '{symbol}' already exists.");

    public static readonly Error ExchangeNotSupported = Error.Problem(
       "Asset.ExchangeNotSupported",
       $"The specified asset type does not support an exchange. Please provide a valid exchange for the asset type.");
}
