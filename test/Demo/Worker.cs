using PushGateway.Client;
using IMeterFactory = PushGateway.Client.IMeterFactory;

namespace Demo;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMeterFactory _meterFactory;

    private static readonly HistogramDescriptor<int> _protoHistogramDescriptor = new
    (
        "Temperature",
        "Temperature in the city",
        "C",
        0,
        [-20, -15, -10, -5, -1, 0, 1, 5, 10, 15, 20, 25, 30]
    );

    public Worker(ILogger<Worker> logger, IMeterFactory meterFactory)
    {
        _logger = logger;
        _meterFactory = meterFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            
            var histogram = _meterFactory.CreateHistogram(_protoHistogramDescriptor);
            histogram.Update(Random.Shared.Next(-50, 50));
        }
    }
}

