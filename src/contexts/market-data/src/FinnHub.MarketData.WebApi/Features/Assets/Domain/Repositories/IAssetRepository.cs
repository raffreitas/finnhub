using FinnHub.MarketData.WebApi.Features.Assets.Domain.Entities;

namespace FinnHub.MarketData.WebApi.Features.Assets.Domain.Repositories;

public interface IAssetRepository
{
    Task<Asset?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string symbol, CancellationToken cancellationToken = default);
}
