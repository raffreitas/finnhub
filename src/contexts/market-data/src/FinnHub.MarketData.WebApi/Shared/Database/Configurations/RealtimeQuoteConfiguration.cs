using FinnHub.MarketData.WebApi.Features.Quotes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MongoDB.Bson;

namespace FinnHub.MarketData.WebApi.Shared.Database.Configurations;

internal sealed class RealtimeQuoteConfiguration : IEntityTypeConfiguration<RealtimeQuote>
{
    public void Configure(EntityTypeBuilder<RealtimeQuote> builder)
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

        builder.OwnsOne(x => x.Price, owned =>
            owned.Property(p => p.Value)
                .IsRequired());

        builder.OwnsOne(x => x.Timestamp, owned =>
            owned.Property(t => t.Value)
                .IsRequired());
        
        builder.Property(x => x.Volume)
            .IsRequired(false);

        builder.OwnsOne(x => x.Change, owned =>
            owned.Property(c => c.Value));

        builder.Property(x => x.ChangePercent)
            .IsRequired(false);
    }
}
