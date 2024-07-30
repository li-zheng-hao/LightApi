namespace LightApi.Gateway.YarpNacosProxy;

public class NacosProxyOptions
{
    /// <summary>
    /// 默认10秒
    /// </summary>
    public TimeSpan RefreshPeriod { get; set; }=TimeSpan.FromSeconds(10);

    /// <summary>
    /// Nacos服务分组名称
    /// </summary>
    public string GroupName { get; set; } = "DEFAULT_GROUP";
}