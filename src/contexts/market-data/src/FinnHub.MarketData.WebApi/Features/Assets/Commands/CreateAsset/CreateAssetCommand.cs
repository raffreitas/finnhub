using FluentValidation.Results;

namespace FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

internal sealed record CreateAssetCommand
{
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public required string Type { get; init; }

    public ValidationResult Validate() => new CreateAssetValidator().Validate(this);
}
