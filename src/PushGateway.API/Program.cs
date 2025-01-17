using Microsoft.AspNetCore.Mvc;
using Prometheus;
using PushGateway.API;
using Serilog;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection;
using PushGateway.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, _, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console());

builder.Services.AddSingleton<IMeterProvider, MeterProvider>();

builder.Services
    .AddOpenApi()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(sgo =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        sgo.IncludeXmlComments(xmlPath);
    })
    .AddMetricServer(mop => mop.Port = 9091)//default port actually
    ;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app
        .UseSwagger()
        .UseSwaggerUI(sop =>
        {
            sop.EnableTryItOutByDefault();
            sop.DisplayRequestDuration();
        });
}

app.UseSerilogRequestLogging();
app.UseHttpMetrics();
app.UseMetricServer();
//app.UseHttpsRedirection();

RouteGroupBuilder pushGateway = app.MapGroup("/PushGateway").WithTags("PushGateway");

pushGateway.MapPost("GaugeInt32"
    , async ([FromBody] GaugeDescriptorInt32 d, [FromServices] IMeterProvider meterProvider) =>
    {
        await Task.Yield();
        CreateAndUpdateGauge(meterProvider, d);
        return Results.Ok();
    })
    .WithName("CreateGaugeInt32");

pushGateway.MapPost("GaugeDouble"
    , async ([FromBody] GaugeDescriptorDouble d, [FromServices] IMeterProvider meterProvider) =>
    {
        await Task.Yield();
        CreateAndUpdateGauge(meterProvider, d);
        return Results.Ok();
    })
    .WithName("CreateGaugeDouble");

pushGateway.MapPost("HistogramInt32"
    , async ([FromBody] HistogramDescriptorInt32 d, [FromServices] IMeterProvider meterProvider) =>
    {
        await Task.Yield();
        CreateAndUpdateHistogram(meterProvider, d);
        return Results.Ok();
    })
    .WithName("CreateHistogramInt32");

pushGateway.MapPost("HistogramDouble"
    , async ([FromBody] HistogramDescriptorDouble d, [FromServices] IMeterProvider meterProvider) =>
    {
        await Task.Yield();
        CreateAndUpdateHistogram(meterProvider, d);
        return Results.Ok();
    })
    .WithName("CreateHistogramDouble");

app.Run();

static void CreateAndUpdateGauge<T>(IMeterProvider meterProvider, GaugeDescriptor<T> d) where T : struct, INumber<T>
{
    var gauge = meterProvider.Meter.CreateUpDownCounter<T>(d.Name, d.Unit, d.Description);
    gauge.Add(d.Delta);
}

static void CreateAndUpdateHistogram<T>(IMeterProvider meterProvider, HistogramDescriptor<T> d) where T : struct, INumber<T>
{
    var histogram = meterProvider.Meter.CreateHistogram<T>(d.Name, d.Unit, d.Description
        , advice: new InstrumentAdvice<T>()
        {
            HistogramBucketBoundaries = d.Boundaries
        });
    histogram.Record(d.Delta);
}

/*
 so far there is a problem with the values - figure it out!
    
expected boundaries are [-20, -15, -10, -5, -1, 0, 1, 5, 10, 15, 20, 25, 30]
observed - see below (le values)
   # HELP pushgatewayapi_temperature (C) Temperature in the city (Histogram`1)
   # TYPE pushgatewayapi_temperature histogram
   pushgatewayapi_temperature_sum 0
   pushgatewayapi_temperature_count 8
   pushgatewayapi_temperature_bucket{le="0.01"} 5
   pushgatewayapi_temperature_bucket{le="0.02"} 5
   pushgatewayapi_temperature_bucket{le="0.04"} 5
   pushgatewayapi_temperature_bucket{le="0.08"} 5
   pushgatewayapi_temperature_bucket{le="0.16"} 5
   pushgatewayapi_temperature_bucket{le="0.32"} 5
   pushgatewayapi_temperature_bucket{le="0.64"} 5
   pushgatewayapi_temperature_bucket{le="1.28"} 5
   pushgatewayapi_temperature_bucket{le="2.56"} 5
   pushgatewayapi_temperature_bucket{le="5.12"} 5
   pushgatewayapi_temperature_bucket{le="10.24"} 5
   pushgatewayapi_temperature_bucket{le="20.48"} 5
   pushgatewayapi_temperature_bucket{le="40.96"} 8
   pushgatewayapi_temperature_bucket{le="81.92"} 8
   pushgatewayapi_temperature_bucket{le="163.84"} 8
   pushgatewayapi_temperature_bucket{le="327.68"} 8
   pushgatewayapi_temperature_bucket{le="655.36"} 8
   pushgatewayapi_temperature_bucket{le="1310.72"} 8
   pushgatewayapi_temperature_bucket{le="2621.44"} 8
   pushgatewayapi_temperature_bucket{le="5242.88"} 8
   pushgatewayapi_temperature_bucket{le="10485.76"} 8
   pushgatewayapi_temperature_bucket{le="20971.52"} 8
   pushgatewayapi_temperature_bucket{le="41943.04"} 8
   pushgatewayapi_temperature_bucket{le="83886.08"} 8
   pushgatewayapi_temperature_bucket{le="167772.16"} 8
   pushgatewayapi_temperature_bucket{le="+Inf"} 8
 */