using FinnHub.PortfolioManagement.Infrastructure.Caching.Abstractions;

using Microsoft.Extensions.Caching.Hybrid;

namespace FinnHub.PortfolioManagement.Infrastructure.Caching.Services;
internal sealed class HybridCacheService(HybridCache hybridCache) : ICacheService
{
    public async ValueTask<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, IEnumerable<string>? tags = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        => await hybridCache.GetOrCreateAsync(key, factory, GetCacheEntryOptions(expiration), tags, cancellationToken);

    public async ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
        => await hybridCache.RemoveAsync(key, cancellationToken);

    public async ValueTask SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        => await hybridCache.SetAsync(key, value, GetCacheEntryOptions(expiration), cancellationToken: cancellationToken);

    private static HybridCacheEntryOptions GetCacheEntryOptions(TimeSpan? expiration = null)
    {
        if (expiration.HasValue && expiration.Value > TimeSpan.Zero)
        {
            return new HybridCacheEntryOptions
            {
                LocalCacheExpiration = expiration.Value,
                Expiration = expiration.Value
            };
        }

        return new HybridCacheEntryOptions();
    }
}
