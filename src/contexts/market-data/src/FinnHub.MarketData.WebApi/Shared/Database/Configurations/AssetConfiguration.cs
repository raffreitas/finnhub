using FinnHub.MarketData.WebApi.Features.Assets.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MongoDB.Bson;

namespace FinnHub.MarketData.WebApi.Shared.Database.Configurations;

internal sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                v => new ObjectId(v.ToString()),
                v => Guid.Parse(v.ToString())
            );

        builder.OwnsOne(x => x.Symbol, owned =>
        {
            owned.Property(s => s.Value)
                .IsRequired()
                .HasMaxLength(10);

            owned.HasIndex(s => s.Value)
                .IsUnique();
        });

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Exchange)
           .IsRequired()
           .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);
    }
}
