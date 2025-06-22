using FinnHub.MarketData.WebApi.Features.Assets.Domain.Entities;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Enums;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Events;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Repositories;
using FinnHub.MarketData.WebApi.Features.Assets.Errors;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Services;
using FinnHub.Shared.Core;
using FinnHub.Shared.Core.Extensions;

namespace FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

internal sealed class CreateAssetHandler(
    IAssetRepository assetRepository,
    IMessageBus messageBus,
    ILogger<CreateAssetHandler> logger
)
{
    public async Task<Result> Handle(CreateAssetCommand command, CancellationToken cancellationToken)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.GetError());


        if (await assetRepository.ExistsAsync(command.Symbol, cancellationToken))
        {
            logger.LogWarning("Asset symbol {AssetSymbol} already exists", command.Symbol);
            return Result.Failure(AssetErrors.AssetSymbolNotUnique(command.Symbol));
        }

        var assetType = Enum.Parse<AssetType>(command.Type, true);

        string? exchange = assetType switch
        {
            AssetType.Crypto => "BINANCE",
            _ => null
        };

        if (string.IsNullOrEmpty(exchange))
            return Result.Failure(AssetErrors.ExchangeNotSupported);

        var asset = new Asset(command.Symbol, command.Name, assetType, exchange);

        await assetRepository.AddAsync(asset, cancellationToken);

        await messageBus.PublishAsync(new AssetWatchlistChangedEvent(
            Guid.NewGuid(),
            exchange,
            AssetChangedType.Added.ToString(),
            asset.Symbol,
            assetType.ToString(),
            DateTimeOffset.UtcNow
        ), cancellationToken: cancellationToken);

        return Result.Success();
    }
}
