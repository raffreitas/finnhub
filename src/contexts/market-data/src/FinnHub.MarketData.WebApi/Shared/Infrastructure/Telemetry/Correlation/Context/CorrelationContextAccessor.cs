namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Context;
internal sealed class CorrelationContextAccessor : ICorrelationContextAccessor
{
    private readonly AsyncLocal<CorrelationContext> _correlationContext = new();

    public CorrelationContext? Context
    {
        get => _correlationContext.Value;
        set => _correlationContext.Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
