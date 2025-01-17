using Microsoft.Extensions.DependencyInjection;

namespace PushGateway.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMeterFactory(this IServiceCollection services, string pushGatewayHost)
    {
        services.AddHttpClient();//registers IHttpClientFactory under the hood
        services.AddSingleton<IMeterFactory>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return new MeterFactory(httpClientFactory, pushGatewayHost);
        });
        return services;
    }
}