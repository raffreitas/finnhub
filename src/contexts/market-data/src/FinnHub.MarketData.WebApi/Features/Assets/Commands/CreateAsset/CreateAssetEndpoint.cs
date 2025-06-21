using FinnHub.MarketData.WebApi.Shared.Presentation.Endpoints;
using FinnHub.Shared.Core.Extensions;

namespace FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

internal sealed class CreateAssetEndpoint : IEndpoint
{
    public sealed record class Request
    {
        public required string Name { get; init; }
        public required string Symbol { get; init; }
        public required string Type { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/assets", async (Request request, CreateAssetHandler handler, CancellationToken cancellationToken) =>
        {
            var command = new CreateAssetCommand
            {
                Name = request.Name,
                Symbol = request.Symbol,
                Type = request.Type
            };

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Created, CustomResults.Problem);
        });
    }
}
