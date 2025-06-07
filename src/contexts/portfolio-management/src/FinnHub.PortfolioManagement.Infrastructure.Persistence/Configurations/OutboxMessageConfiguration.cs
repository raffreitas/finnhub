using System.Text.Json;
using System.Text.Json.Serialization;

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
            .HasConversion(
                @event => JsonSerializer.Serialize(@event, JsonOptions),
                @event => JsonSerializer.Deserialize<object>(@event, JsonOptions) ?? string.Empty
            )
            .IsRequired();
        builder
            .Property(e => e.Headers)
            .HasConversion(
                @event => JsonSerializer.Serialize(@event, JsonOptions),
                @event => JsonSerializer.Deserialize<object>(@event, JsonOptions) ?? string.Empty
            )
            .IsRequired(false);

        builder
            .Property(e => e.DeliveryAttempts)
            .IsRequired();
        builder
            .Property(e => e.ErrorMessage)
            .IsRequired(false);

        builder.HasIndex(e => new { e.ProcessedAt, e.CreatedAt });
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
}
