using LightApi.Gateway.GrayLoadBalancing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Nacos.V2;
using Nacos.V2.Naming.Dtos;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.ServiceDiscovery;

namespace LightApi.Gateway.YarpNacosProxy;

public class NacosDestinationResolver:IDestinationResolver
{
    private readonly IOptionsMonitor<NacosProxyOptions> _options;
    private readonly INacosNamingService _nacosNamingService;
    private readonly NacosServiceStore _nacosServiceStore;

    public NacosDestinationResolver(IOptionsMonitor<NacosProxyOptions> options,INacosNamingService nacosNamingService,NacosServiceStore nacosServiceStore)
    {
        _options = options;
        _nacosNamingService = nacosNamingService;
        _nacosServiceStore = nacosServiceStore;
    }
    
    public async ValueTask<ResolvedDestinationCollection> ResolveDestinationsAsync(IReadOnlyDictionary<string, DestinationConfig> destinations, CancellationToken cancellationToken)
    {
        var options = _options.CurrentValue;
        Dictionary<string, DestinationConfig> results = new();
        var tasks = new List<Task<List<(string Name, DestinationConfig Config)>>>(destinations.Count);
        foreach (var (destinationId, destinationConfig) in destinations)
        {
            tasks.Add(ResolveHostAsync(options, destinationId, destinationConfig, cancellationToken));
        }

        await Task.WhenAll(tasks);
        foreach (var task in tasks)
        {
            var configs = await task;
            foreach (var (name, config) in configs)
            {
                Console.WriteLine($"[{name}]-[{config.Address}]-[{config.Host}]");
                results[name] = config;
            }
        }

        var changeToken = options.RefreshPeriod switch
        {
            { } refreshPeriod when refreshPeriod > TimeSpan.Zero => new CancellationChangeToken(new CancellationTokenSource(refreshPeriod).Token),
            _ => null,
        };

        return new ResolvedDestinationCollection(results, changeToken);
    }

    private async Task<List<(string Name, DestinationConfig Config)>> ResolveHostAsync(NacosProxyOptions options
        ,  string destinationId,
        DestinationConfig destinationConfig,
        CancellationToken cancellationToken)
    {
        var originalUri = new Uri(destinationConfig.Address);
        var originalHost = destinationConfig.Host is { Length: > 0 } host ? host : originalUri.Authority;

        var instances = await _nacosNamingService.GetAllInstances(destinationId, _options.CurrentValue.GroupName);
        
        
        // 没有返回空的
        instances ??= new List<Instance>();
        
        _nacosServiceStore.Update(destinationId,instances);
        
        var results = new List<(string Name, DestinationConfig Config)>(instances.Count);
        var uriBuilder = new UriBuilder(originalUri);
        var healthUri = destinationConfig.Health is { Length: > 0 } health ? new Uri(health) : null;
        var healthUriBuilder = healthUri is { } ? new UriBuilder(healthUri) : null;
        foreach (var address in instances)
        {
            var addressString = address.Ip;
            uriBuilder.Host = address.Ip;
            uriBuilder.Port = address.Port;
            var resolvedAddress = uriBuilder.Uri.ToString();
            var healthAddress = destinationConfig.Health;
            if (healthUriBuilder is not null)
            {
                healthUriBuilder.Host = addressString;
                healthAddress = healthUriBuilder.Uri.ToString();
            }

            var name = $"[{destinationId}][{uriBuilder.Host}]";
            var config = destinationConfig with { Host = originalHost, Address = resolvedAddress, Health = healthAddress };
            results.Add((name, config));
        }
        
        
        return results;
    }
}


