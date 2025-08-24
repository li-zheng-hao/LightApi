namespace LightApi.Infra.Rpc;

public class RpcOptions
{
    public ServiceDiscoveryType ServiceDiscoveryType { get; set; } = ServiceDiscoveryType.None;

    /// <summary>
    /// 示例：127.0.0.1:8080
    /// </summary>
    public string Host { get; set; } = string.Empty;

    public bool UseTls { get; set; } = false;

    /// <summary>
    /// 示例：DEFAULT_GROUP 不使用Nacos时，此值无效
    /// </summary>
    public string? NacosGroupName { get; set; }
    /// <summary>
    /// nacos缓存时间
    /// </summary>
    public int NacosCacheSeconds { get; set; } = 5;
    /// <summary>
    /// 是否使用默认的标准化重试处理程序
    /// </summary>
    public bool UseStandardResilienceHandler { get; set; } = true;
}

public enum ServiceDiscoveryType
{
    None,
    Nacos,
}
