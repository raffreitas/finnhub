using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Entities;
using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Repositories;

using MongoDB.Driver;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Repositories;

internal sealed class QuoteRepository(IMongoDatabase database) : IQuoteRepository
{
    private readonly IMongoCollection<HistoricalQuote> _collection = database.GetCollection<HistoricalQuote>("historical_quote");
    public async Task AddAsync(HistoricalQuote quote, CancellationToken cancellationToken = default)
        => await _collection.InsertOneAsync(quote, cancellationToken: cancellationToken);

    public async Task<IEnumerable<HistoricalQuote>> GetBySymbolAsync(
        string symbol,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default
    )
    {
        var builder = Builders<HistoricalQuote>.Filter;
        var filter = builder.Eq(x => x.AssetSymbol, symbol);

        if (startDate.HasValue)
            filter &= builder.Gte(x => x.Date, startDate);

        if (endDate.HasValue)
            filter &= builder.Gte(x => x.Date, endDate);

        var skip = (page - 1) * pageSize;

        return await _collection
            .Find(filter)
            .SortByDescending(x => x.Date)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<HistoricalQuote?> GetLatestBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
        => await _collection
            .Find(x => x.AssetSymbol == symbol)
            .SortByDescending(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);
}
