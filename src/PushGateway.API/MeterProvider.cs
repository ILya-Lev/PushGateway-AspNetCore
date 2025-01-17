using System.Diagnostics.Metrics;

namespace PushGateway.API;

public interface IMeterProvider
{
    Meter Meter { get; }
}

public class MeterProvider : IMeterProvider
{
    public Meter Meter { get; } = new Meter("PushGatewayAPI", "1.0");
}