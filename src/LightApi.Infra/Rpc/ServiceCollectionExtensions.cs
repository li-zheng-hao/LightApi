using System.Threading.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Resilience;
using Nacos.V2;
using Refit;

namespace LightApi.Infra.Rpc;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加Refit RPC客户端
    /// </summary>
    /// <typeparam name="T">RPC客户端接口</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="options">RPC配置</param>
    /// <param name="httpClientBuilder">HTTP客户端构建器，用于添加自定义的HTTP客户端</param>
    /// <returns>服务集合</returns>
    /// <exception cref="ArgumentException">当RpcOptions.Host为空时抛出</exception>
    public static IServiceCollection AddRefitRpcClient<T>(this IServiceCollection services, Action<RpcOptions> options, Action<IHttpClientBuilder>? httpClientBuilder = null) where T : class
    {
        var rpcOptions = new RpcOptions();
        options.Invoke(rpcOptions);
        if (string.IsNullOrEmpty(rpcOptions.Host))
        {
            throw new ArgumentException("RpcOptions.Host is required");
        }
        var builder = services.AddRefitClient<T>()
            .ConfigureHttpClient(c =>
        {
            string url = rpcOptions.UseTls ? $"https://{rpcOptions.Host}" : $"http://{rpcOptions.Host}";
            c.BaseAddress = new Uri(url);
        });
        if (rpcOptions.UseStandardResilienceHandler)
        {
            builder.AddStandardResilienceHandler();
        }
        if (rpcOptions.ServiceDiscoveryType == ServiceDiscoveryType.Nacos)
        {
            services.AddMemoryCache();
            services.TryAddTransient<NacosServiceDiscoveryHandler>(sp =>
            {
                var memoryCache = sp.GetRequiredService<IMemoryCache>();
                return new NacosServiceDiscoveryHandler(rpcOptions, sp.GetRequiredService<INacosNamingService>(), memoryCache);
            });
            builder.AddHttpMessageHandler<NacosServiceDiscoveryHandler>();
        }
        if (httpClientBuilder != null)
        {
            httpClientBuilder.Invoke(builder);
        }
        return services;
    }
}
