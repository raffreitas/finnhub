namespace FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

public sealed record CreateAssetCommand
{
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public required string Type { get; init; }
}
