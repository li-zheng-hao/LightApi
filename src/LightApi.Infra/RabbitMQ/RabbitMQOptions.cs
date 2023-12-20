namespace LightApi.Infra.RabbitMQ;

public class RabbitMqOptions
{
    /// <summary>
    /// 配置路径
    /// </summary>
    public const string Section = "RabbitMq";
    
    public List<RabbitMqConfig>? Configs { get; set; }
}

public class RabbitMqConfig
{
    public string Key { get; set; }
    
    public string ConnectionString { get; set; }
}