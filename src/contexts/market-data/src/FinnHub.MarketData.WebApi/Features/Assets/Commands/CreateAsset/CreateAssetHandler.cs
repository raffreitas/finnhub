using FinnHub.MarketData.WebApi.Features.Assets.Entities;
using FinnHub.MarketData.WebApi.Shared.Domain.Enums;
using FinnHub.MarketData.WebApi.Shared.Domain.ValueObjects;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Database.Context;

namespace FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

internal sealed class CreateAssetHandler(ApplicationDbContext dbContext)
{
    public async Task Handle(CreateAssetCommand command, CancellationToken cancellationToken)
    {
        var assetSymbol = new AssetSymbol(command.Symbol);
        var assetType = Enum.Parse<AssetType>(command.Type, true);

        var exchange = assetType switch
        {
            AssetType.Crypto => "BINANCE",
            _ => throw new ArgumentException(nameof(assetType))
        };

        var asset = new Asset(assetSymbol, command.Name, assetType, exchange);

        dbContext.Assets.Add(asset);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
