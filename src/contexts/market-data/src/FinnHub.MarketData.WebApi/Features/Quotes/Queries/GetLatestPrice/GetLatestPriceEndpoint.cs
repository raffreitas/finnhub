using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Repositories;
using FinnHub.MarketData.WebApi.Shared.Presentation.Endpoints;

using Microsoft.AspNetCore.Mvc;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Queries.GetLatestPrice;

public class GetLatestPriceEndpoint : IEndpoint
{
    internal sealed record Response(decimal CurrentPrice);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1/quotes/{assetSymbol}/current-price",
            async (
                [FromRoute] string assetSymbol,
                [FromServices] IQuoteRepository quoteRepository,
                CancellationToken cancellationToken
            ) =>
            {
                var latestQuote = await quoteRepository.GetLatestBySymbolAsync(assetSymbol, cancellationToken);

                return latestQuote is null
                    ? Results.NotFound()
                    : Results.Ok(new Response(latestQuote.Close));
            })
            .RequireAuthorization()
            .Produces<Response>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(EndpointTags.Quotes);
    }
}
