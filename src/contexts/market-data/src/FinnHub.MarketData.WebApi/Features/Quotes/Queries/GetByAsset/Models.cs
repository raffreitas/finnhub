namespace FinnHub.MarketData.WebApi.Features.Quotes.Queries.GetByAsset;

public sealed record Response(decimal CurrentPrice);

public sealed record Request(string AssetSymbol);