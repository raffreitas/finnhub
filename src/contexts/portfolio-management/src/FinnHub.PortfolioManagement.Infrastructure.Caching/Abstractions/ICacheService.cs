namespace FinnHub.PortfolioManagement.Infrastructure.Caching.Abstractions;
public interface ICacheService
{
    ValueTask<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, IEnumerable<string>? tags = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    ValueTask SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);
}
