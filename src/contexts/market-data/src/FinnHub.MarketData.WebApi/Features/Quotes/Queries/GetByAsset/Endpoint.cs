using FastEndpoints;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Queries.GetByAsset;

public sealed class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/quotes/{AssetSymbol}");
        Version(1);
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await SendNotFoundAsync(cancellation: ct);
    }
}
