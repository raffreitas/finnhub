using FinnHub.PortfolioManagement.Domain.Aggregates;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Configurations;
internal sealed class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => new { p.UserId, p.Name })
            .IsUnique();

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(x => x.CreationDate)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasMany(p => p.Positions)
            .WithOne()
            .HasForeignKey(p => p.PortfolioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Transactions)
            .WithOne()
            .HasForeignKey(p => p.PortfolioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
