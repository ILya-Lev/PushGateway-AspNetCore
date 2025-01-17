using System.Net.Http.Json;
using System.Numerics;

namespace PushGateway.Client;

public interface IHistogramMetric<T> where T : struct, INumber<T>
{
    void Update(T delta);
}

public class HistogramMetric<T> : IHistogramMetric<T> where T : struct, INumber<T>
{
    private readonly HttpClient _client;
    private readonly HistogramDescriptor<T> _descriptor;
    private readonly string _route;

    public HistogramMetric(HttpClient client, HistogramDescriptor<T> descriptor)
    {
        _client = client;
        _descriptor = descriptor;
        _route = "PushGateway/Histogram" + typeof(T).Name;
    }
    public void Update(T delta)
    {
        _client.PostAsJsonAsync(_route, _descriptor with { Delta = delta });
    }
}