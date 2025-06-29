using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Context;

namespace FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Factory;

internal sealed class CorrelationContextFactory(ICorrelationContextAccessor correlationContextAccessor) : ICorrelationContextFactory
{
    private readonly Lock _lock = new();

    public CorrelationContext Create(string correlationId)
    {
        lock (_lock)
        {
            var context = new CorrelationContext(correlationId);
            return correlationContextAccessor.Context = context;
        }
    }

    public CorrelationContext Create() => Create(Guid.NewGuid().ToString("N"));
}
