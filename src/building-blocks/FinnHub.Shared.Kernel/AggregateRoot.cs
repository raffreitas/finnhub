namespace FinnHub.Shared.Kernel;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyList<DomainEvent> DomainEvents
        => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent eventItem)
        => _domainEvents.Add(eventItem);

    protected void RemoveDomainEvent(DomainEvent eventItem)
        => _domainEvents.Remove(eventItem);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
