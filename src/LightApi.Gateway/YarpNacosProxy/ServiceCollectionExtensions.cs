using Yarp.ReverseProxy.ServiceDiscovery;

namespace LightApi.Gateway.YarpNacosProxy;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYarpNacosProxy(this IServiceCollection services)
    {
        services.AddSingleton<IDestinationResolver, NacosDestinationResolver>();
        return services;
    }
    
    public static IReverseProxyBuilder AddNacosDestinationResolver(this IReverseProxyBuilder builder, Action<NacosProxyOptions>? configureOptions = null)
    {
        builder.Services.AddSingleton<IDestinationResolver, NacosDestinationResolver>();
        if (configureOptions is not null)
        {
            builder.Services.Configure(configureOptions);
        }

        return builder;
    }
}