using FinnHub.MarketData.WebApi.Features.Quotes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MongoDB.Bson;

namespace FinnHub.MarketData.WebApi.Shared.Database.Configurations;

internal sealed class HistoricalQuoteConfiguration : IEntityTypeConfiguration<HistoricalQuote>
{
    public void Configure(EntityTypeBuilder<HistoricalQuote> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                v => new ObjectId(v.ToString()),
                v => Guid.Parse(v.ToString())
            );

        builder.OwnsOne(x => x.AssetSymbol, owned => owned
            .Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(10));

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.Interval)
            .IsRequired()
            .HasMaxLength(10);

        builder.OwnsOne(x => x.Data, data =>
        {
            data.Property(d => d.Open).IsRequired();
            data.Property(d => d.High).IsRequired();
            data.Property(d => d.Low).IsRequired();
            data.Property(d => d.Close).IsRequired();
            data.Property(d => d.Volume).IsRequired();
        });
    }
}
