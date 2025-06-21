namespace FinnHub.Shared.Kernel;

public abstract record DomainEvent(Guid AggregateId)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AggregateId { get; } = AggregateId;
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    protected abstract string EventVersion { get; }
}
