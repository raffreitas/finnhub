using FinnHub.MarketData.WebApi.Features.Assets.Domain.Entities;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Enums;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Repositories;

namespace FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

internal sealed class CreateAssetHandler(
    IAssetRepository assetRepository,
    ILogger<CreateAssetHandler> logger
)
{
    public async Task Handle(CreateAssetCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
            throw new Exception(validationResult.Errors.Select(x => x.ErrorMessage).First());


        if (await assetRepository.ExistsAsync(command.Symbol, cancellationToken))
        {
            logger.LogWarning("Asset symbol {AssetSymbol} already exists", command.Symbol);
            throw new InvalidOperationException($"Asset with symbol '{command.Symbol}' already exists.");
        }

        var assetType = Enum.Parse<AssetType>(command.Type, true);

        var exchange = assetType switch
        {
            AssetType.Crypto => "BINANCE",
            _ => throw new ArgumentOutOfRangeException($"Exchange for asset type '{assetType}' is not supported.")
        };

        var asset = new Asset(command.Symbol, command.Name, assetType, exchange);

        await assetRepository.AddAsync(asset, cancellationToken);
    }
}
