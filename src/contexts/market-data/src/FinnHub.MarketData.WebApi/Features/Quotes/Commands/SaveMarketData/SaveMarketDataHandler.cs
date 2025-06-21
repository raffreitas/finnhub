using FinnHub.MarketData.WebApi.Features.Quotes.Commands.IngestMarketData;
using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Entities;
using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Repositories;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Commands.SaveMarketData;

internal sealed class SaveMarketDataHandler(IQuoteRepository quoteRepository)
{
    public async Task Handle(SaveMarketDataCommand command, CancellationToken cancellationToken)
    {
        var quote = new HistoricalQuote(
            command.Symbol,
            DateOnly.FromDateTime(command.Timestamp.Date),
            command.OpenPrice,
            command.HighPrice,
            command.LowPrice,
            command.LastPrice,
            (long)command.Volume,
            "1d"
        );

        await quoteRepository.AddAsync(quote, cancellationToken);
    }
}
