using LightApi.Infra.Rpc;
using Nacos.V2;
using Nacos.V2.Common;
using Nacos.V2.Naming.Dtos;

namespace LightApi.Infra.Rpc;

public class NacosServiceDiscoveryHandler : DelegatingHandler
{
    private readonly RpcOptions _rpcOptions;
    private readonly INacosNamingService _nacosNamingService;

    public NacosServiceDiscoveryHandler(RpcOptions rpcOptions, INacosNamingService nacosNamingService)
    {
        _rpcOptions = rpcOptions;
        _nacosNamingService = nacosNamingService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var serviceName = _rpcOptions!.Host;
        string groupName = _rpcOptions.NacosGroupName ?? Constants.DEFAULT_GROUP;
        var serviceInstances = await _nacosNamingService.SelectInstances(serviceName, groupName, healthy: true, subscribe: true);
        if (serviceInstances.Count == 0)
        {
            throw new HttpRequestException($"No service instances found for {serviceName}");
        }
        var serviceInstance = GetWeightBalancedServiceInstance(serviceInstances);
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
