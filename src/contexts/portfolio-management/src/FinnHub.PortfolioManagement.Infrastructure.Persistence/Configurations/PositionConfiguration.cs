using FinnHub.PortfolioManagement.Domain.Aggregates.Entities;
using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Configurations;

internal sealed class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.AssetSymbol)
            .HasConversion(
                v => v.Value,
                v => AssetSymbol.Create(v)
            )
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.Quantity)
            .HasConversion(
                v => v.Value,
                v => Quantity.Create(v)
            )
            .IsRequired()
            .HasMaxLength(10);

        builder.ComplexProperty(p => p.AverageCost, averageCost =>
        {
            averageCost.Property(m => m.Value)
                .HasPrecision(18, 2)
                .IsRequired();

            averageCost.Property(m => m.Currency)
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.CurrentMarketPrice, c =>
        {
            c.Property(m => m.Value)
                .HasPrecision(18, 2)
                .IsRequired();

            c.Property(m => m.Currency)
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(x => x.LastUpdated)
            .IsRequired();

        builder.Property(p => p.PortfolioId)
            .IsRequired();
    }
}
