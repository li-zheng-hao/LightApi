using LightApi.Gateway.GrayLoadBalancing;
using Nacos.V2.Naming.Dtos;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

namespace LightApi.Gateway.LoadBalance;

/// <summary>
/// 灰度+权重负载均衡策略
/// </summary>
public class GrayWeightLoadBalancingPolicy:ILoadBalancingPolicy
{
    private readonly NacosServiceStore _nacosServiceStore;
    private readonly ILogger<GrayWeightLoadBalancingPolicy> _logger;

    public GrayWeightLoadBalancingPolicy(NacosServiceStore nacosServiceStore,ILogger<GrayWeightLoadBalancingPolicy> logger)
    {
        _nacosServiceStore = nacosServiceStore;
        _logger = logger;
    }

    public DestinationState? PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
    {
        if (cluster.Destinations.IsEmpty) return null;

        var success=_nacosServiceStore.Services.TryGetValue(cluster.ClusterId, out var services);
        if (!success) return null;
        
        var instance = Filter(context,services!);
        if (instance == null) return null;
        var target=cluster.Destinations.First().Value;
        var targetUri=new Uri(target.Model.Config.Address);
        return new DestinationState(target.DestinationId,
            new DestinationModel(new DestinationConfig()
            {
                Address = $"{targetUri.Scheme}://{instance.Ip}:{instance.Port}/",
            })
        );
    }

    public Instance? Filter(HttpContext httpContext,List<Instance> instances)
    {
        var realServices = instances.Select(it => it).Where(it => it!.Weight > 0).ToList();

        // 如果是灰度服务，优先找灰度节点
        if (httpContext.Request.Headers.ContainsKey("Gray"))
        {
            var grayServices = realServices.Where(it => it!.Metadata?.ContainsKey("Gray") == true).ToList();
            var target = WeigtRoubin(grayServices);

            if (target != null)
                return target;
            
            // 找不到灰度节点或不是灰度请求 找所有节点
            target = WeigtRoubin(realServices);

            if (target == null)
            {
                _logger.LogInformation("当前为灰度请求，但是未找到任何可用服务");
                return null;
            }

            return target;
        }
        
        // 找不到灰度节点或不是灰度请求 找所有节点
        var realTarget = WeigtRoubin(realServices.Where(it => it!.Metadata?.ContainsKey("Gray") == false).ToList());
    
        if (realTarget == null)
            _logger.LogWarning("未找到任何可用服务");
        
        return realTarget;
    }
    /// <summary>
    /// 根据权重随机选择服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private Instance? WeigtRoubin(List<Instance> services)
    {
        if (!services.Any()) return null;

        var totalWeight = services.Sum(s => s.Weight);
        var randomNumber = new Random().NextDouble() * totalWeight;
        var currentWeight = 0.0;
        foreach (var service in services)
        {
            currentWeight += service.Weight;
            if (currentWeight >= randomNumber)
            {
                return service;
            }
        }

        return null;
    }
    
    public string Name { get; } = "Gray";
}