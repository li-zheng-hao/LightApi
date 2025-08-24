using LightApi.Infra.Rpc;
using Microsoft.Extensions.Caching.Memory;
using Nacos.V2;
using Nacos.V2.Common;
using Nacos.V2.Naming.Dtos;

namespace LightApi.Infra.Rpc;

public class NacosServiceDiscoveryHandler : DelegatingHandler
{
    private readonly RpcOptions _rpcOptions;
    private readonly INacosNamingService _nacosNamingService;
    private readonly IMemoryCache _memoryCache;

    private const string ServiceInstancesCacheKey = "NACOS_SERVICE_INSTANCES:";
    public NacosServiceDiscoveryHandler(RpcOptions rpcOptions, INacosNamingService nacosNamingService, IMemoryCache memoryCache)
    {
        _rpcOptions = rpcOptions;
        _nacosNamingService = nacosNamingService;
        _memoryCache = memoryCache;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var serviceName = _rpcOptions!.Host;
        string groupName = _rpcOptions.NacosGroupName ?? Constants.DEFAULT_GROUP;
        string name = $"{groupName}:{serviceName}";
        var serviceInstances = await _memoryCache.GetOrCreateAsync(ServiceInstancesCacheKey + name, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_rpcOptions.NacosCacheSeconds);
            return await _nacosNamingService.SelectInstances(serviceName, groupName, healthy: true, subscribe: false);
        });
        if (serviceInstances?.Count == 0)
        {
            throw new HttpRequestException($"No service instances found for {serviceName}");
        }
        var serviceInstance = GetWeightBalancedServiceInstance(serviceInstances!);
        var originalUri = request.RequestUri;
        var newUri = new UriBuilder(originalUri!)
        {
            Host = serviceInstance.Ip,
            Port = serviceInstance.Port
        }.Uri;
        request.RequestUri = newUri!;
        return await base.SendAsync(request, cancellationToken);
    }


    private Instance GetWeightBalancedServiceInstance(List<Instance> serviceInstances)
    {
        var totalWeight = serviceInstances.Sum(s => s.Weight);
        var random = new Random();
        var randomWeight = random.Next(0, (int)totalWeight);
        var currentWeight = 0;
        foreach (var serviceInstance in serviceInstances)
        {
            currentWeight += (int)serviceInstance.Weight;
            if (randomWeight <= currentWeight)
            {
                return serviceInstance;
            }
        }
        return serviceInstances[0];
    }
}
