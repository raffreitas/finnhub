using FinnHub.MarketData.WebApi.Features.Quotes.Commands.SaveMarketData;
using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Repositories;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Repositories;

namespace FinnHub.MarketData.WebApi.Features.Quotes;

public static class DependencyInjection
{
    public static IServiceCollection AddQuotesFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IQuoteRepository, QuoteRepository>();

        services.AddScoped<SaveMarketDataHandler>();

        return services;
    }
}
