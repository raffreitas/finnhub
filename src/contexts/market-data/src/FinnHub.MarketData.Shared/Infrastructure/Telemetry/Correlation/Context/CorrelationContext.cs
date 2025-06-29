namespace FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Context;
public class CorrelationContext
{
    public string CorrelationId { get; }

    public CorrelationContext(string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        CorrelationId = correlationId;
    }
}
