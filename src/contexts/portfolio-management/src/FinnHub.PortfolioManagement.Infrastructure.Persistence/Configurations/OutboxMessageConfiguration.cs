using FinnHub.PortfolioManagement.Infrastructure.Messaging.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .IsRequired();
        builder
            .Property(e => e.CreatedAt)
            .IsRequired();
        builder
            .Property(e => e.ProcessedAt)
            .IsRequired(false);
        builder
            .Property(e => e.EventName)
            .IsRequired();
        builder
            .Property(e => e.MessageType)
            .IsRequired();
        builder
            .Property(e => e.MessageContent)
            .IsRequired();
        builder
            .Property(e => e.Headers)
            .IsRequired(false);

        builder
            .Property(e => e.DeliveryAttempts)
            .IsRequired();
        builder
            .Property(e => e.ErrorMessage)
            .IsRequired(false);

        builder.HasIndex(e => new { e.ProcessedAt, e.CreatedAt });
    }
}
