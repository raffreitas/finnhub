using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Entities;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Domain.Repositories;

public interface IQuoteRepository
{
    Task<IEnumerable<HistoricalQuote>> GetBySymbolAsync(
        string symbol,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default
    );

    Task<HistoricalQuote?> GetLatestBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    Task AddAsync(HistoricalQuote quote, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string symbol, DateOnly date, CancellationToken cancellationToken = default);
}
