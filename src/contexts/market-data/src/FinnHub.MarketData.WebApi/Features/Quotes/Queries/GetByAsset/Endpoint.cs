using FinnHub.MarketData.WebApi.Shared.Endpoints;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Queries.GetByAsset;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/quotes/{assetSymbol}", (string assetSymbol) => Results.NotFound()).RequireAuthorization();
    }
}
