using System.Reflection;
using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Topology;

namespace LightApi.Infra.RabbitMQ;

/// <summary>
/// RabbitMQ工具类
/// </summary>
public class RabbitMqManager
{
    private Dictionary<string, IBus> Buses { get; set; } = new();

    /// <summary>
    /// 获取RabbitMQ连接
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IBus GetBus(string key = "default")
    {
        return Buses.TryGetValue(key!, out var bus) ? bus : throw new Exception($"未找到key为{key}的RabbitMQ连接");
    }
    
    /// <summary>
    /// 添加RabbitMQ连接
    /// </summary>
    /// <param name="bus"></param>
    /// <param name="key"></param>
    public void AddBus(IBus bus, string key = "default")
    {
        Buses.Add(key, bus);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="msg"></param>
    /// <param name="busKey"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task PublishAsync<T>(string topic, T msg, string busKey = "default")
    {
        var transportMsg = new Message<T>(msg);
        return GetBus(busKey).Advanced.PublishAsync<T>(new Exchange("amq.direct"), topic, false, transportMsg);
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="handler"></param>
    /// <param name="queueName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="busKey"></param>
    /// <typeparam name="T"></typeparam>
    public async Task SubscribeAsync<T>(Action<SubscribeOption<T>> configure)
    {
        SubscribeOption<T> option = new();
        configure(option);
        if (option.QueueName == null)
            option.QueueName = Assembly.GetEntryAssembly()!.GetName().Name;
      
        Action<IQueueDeclareConfiguration> queueConfigure=configure =>
        {
            configure.AsDurable(true);
            configure.AsExclusive(false);
            configure.AsAutoDelete(false);
            configure.WithArgument("x-message-ttl", 1000 * 60 * 60 * 24 * 7);
        };
        if(option.QueueConfigure!=null)
            queueConfigure = option.QueueConfigure;
        var queue = await GetBus(option.BusKey).Advanced.QueueDeclareAsync(option.QueueName, 
            queueConfigure);
        await GetBus(option.BusKey).Advanced.BindAsync(new Exchange(option.ExchangeName), queue, option.Topic);
        GetBus(option.BusKey).Advanced.Consume(queue, x => x
            .Add<T>((message, info) => option.Handler(message.Body, info))
        );
    }
}