using FinnHub.MarketData.WebApi.Features.Assets.Entities;
using FinnHub.MarketData.WebApi.Features.Quotes.Entities;
using FinnHub.MarketData.WebApi.Shared.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace FinnHub.MarketData.WebApi.Shared.Database.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<HistoricalQuote> HistoricalQuotes { get; set; }
    public DbSet<RealtimeQuote> RealtimeQuotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<IDomainEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
