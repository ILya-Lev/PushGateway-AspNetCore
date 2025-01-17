using System.Net.Http.Json;
using System.Numerics;

namespace PushGateway.Client;

public interface IGaugeMetric<T> where T : struct, INumber<T>
{
    void Update(T delta);
}

public class GaugeMetric<T> : IGaugeMetric<T> where T : struct, INumber<T>
{
    private readonly HttpClient _client;
    private readonly GaugeDescriptor<T> _descriptor;
    private readonly string _route;

    public GaugeMetric(HttpClient client, GaugeDescriptor<T> descriptor)
    {
        _client = client;
        _descriptor = descriptor;
        _route = "PushGateway/Gauge" + typeof(T).Name;
    }
    public void Update(T delta)
    {
        _client.PostAsJsonAsync(_route, _descriptor with { Delta = delta });
    }
}