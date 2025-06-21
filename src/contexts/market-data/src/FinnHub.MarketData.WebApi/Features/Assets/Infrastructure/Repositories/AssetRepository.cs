using FinnHub.MarketData.WebApi.Features.Assets.Domain.Entities;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Repositories;

using MongoDB.Driver;

namespace FinnHub.MarketData.WebApi.Features.Assets.Infrastructure.Repositories;

internal sealed class AssetRepository(IMongoDatabase database) : IAssetRepository
{
    private readonly IMongoCollection<Asset> _collection = database.GetCollection<Asset>("assets");

    public async Task<Asset?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
        => await _collection
            .Find(x => x.Symbol == symbol)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Asset>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        => await _collection
            .Find(x => x.IsActive)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
        => await _collection.InsertOneAsync(asset, cancellationToken: cancellationToken);


    public async Task<bool> ExistsAsync(string symbol, CancellationToken cancellationToken = default)
        => await _collection
             .Find(x => x.IsActive && x.Symbol == symbol)
             .AnyAsync(cancellationToken);
}
