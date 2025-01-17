using Demo;
using PushGateway.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddMeterFactory("http://localhost:5273");

var host = builder.Build();
host.Run();
