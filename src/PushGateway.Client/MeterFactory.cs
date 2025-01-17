using System.Numerics;

namespace PushGateway.Client;

public interface IMeterFactory
{
    IGaugeMetric<T> CreateGauge<T>(GaugeDescriptor<T> descriptor) where T : struct, INumber<T>;
    IHistogramMetric<T> CreateHistogram<T>(HistogramDescriptor<T> descriptor) where T : struct, INumber<T>;
}

internal class MeterFactory : IMeterFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseAddress;

    public MeterFactory(IHttpClientFactory httpClientFactory, string baseAddress)
    {
        _httpClientFactory = httpClientFactory;
        _baseAddress = baseAddress;
    }

    public IGaugeMetric<T> CreateGauge<T>(GaugeDescriptor<T> descriptor) where T : struct, INumber<T>
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_baseAddress);
        return new GaugeMetric<T>(client, descriptor);
    }

    public IHistogramMetric<T> CreateHistogram<T>(HistogramDescriptor<T> descriptor) where T : struct, INumber<T>
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_baseAddress);
        return new HistogramMetric<T>(client, descriptor);
    }
}

