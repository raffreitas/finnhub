using FinnHub.PortfolioManagement.Domain.Aggregates.Entities;
using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.AssetSymbol)
            .HasConversion(
                v => v.Value,
                v => AssetSymbol.Create(v)
            )
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.IsSettled)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .HasConversion(
                v => v.Value,
                v => Quantity.Create(v)
            )
            .IsRequired()
            .HasMaxLength(10);

        builder.ComplexProperty(p => p.Price, averageCost =>
        {
            averageCost.Property(m => m.Value)
                .HasPrecision(18, 2)
                .IsRequired();

            averageCost.Property(m => m.Currency)
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.CurrentMarketValue, c =>
        {
            c.Property(m => m.Value)
                .HasPrecision(18, 2)
                .IsRequired();

            c.Property(m => m.Currency)
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}
