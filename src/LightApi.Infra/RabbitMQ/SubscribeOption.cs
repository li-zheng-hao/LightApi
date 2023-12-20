using EasyNetQ;
using EasyNetQ.Consumer;

namespace LightApi.Infra.RabbitMQ;

public class SubscribeOption<T>
{
    public string Topic { get; set; }
    public Func<T, MessageReceivedInfo, Task<AckStrategy>> Handler { get; set; }
    public string? QueueName { get; set; }
    public string ExchangeName { get; set; } = "amq.direct";
    public string BusKey { get; set; } = "default";

    /// <summary>
    /// 消费者队列配置 不设置使用默认参数
    /// configure.AsDurable(true);
    /// configure.AsExclusive(false);
    /// configure.AsAutoDelete(false);
    /// configure.WithArgument("x-message-ttl", 1000 * 60 * 60 * 24 * 7);
    /// </summary>
    public Action<IQueueDeclareConfiguration>? QueueConfigure { get; set; }
}