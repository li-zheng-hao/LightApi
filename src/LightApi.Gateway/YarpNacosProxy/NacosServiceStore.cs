using Nacos.V2.Naming.Dtos;

namespace LightApi.Gateway.GrayLoadBalancing;

public class NacosServiceStore
{
    public NacosServiceStore()
    {
        Services = new Dictionary<string, List<Instance>>();
    }
    /// <summary>
    /// key是服务名，value是服务实例
    /// </summary>
    public Dictionary<string,List<Instance>> Services { get; set; }
    
    public void Update(string serviceName,List<Instance> services)
    {
        Services[serviceName] = services;
    }
}