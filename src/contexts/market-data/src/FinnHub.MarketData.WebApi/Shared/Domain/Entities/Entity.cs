namespace FinnHub.MarketData.WebApi.Shared.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
}
