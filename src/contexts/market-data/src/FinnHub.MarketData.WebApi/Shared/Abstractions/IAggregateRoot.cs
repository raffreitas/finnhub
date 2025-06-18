namespace FinnHub.MarketData.WebApi.Shared.Abstractions;

public interface IAggregateRoot
{
    Guid Id { get; protected init; }
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
