namespace FinnHub.Shared.Kernel;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
