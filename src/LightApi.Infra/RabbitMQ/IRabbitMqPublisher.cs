using RabbitMQ.Client;

namespace LightApi.Infra.RabbitMQ;

public interface IRabbitMqPublisher
{
    Task InitConnection(IConnection connection);

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="routingKey"></param>
    /// <param name="message"></param>
    /// <param name="exchange"></param>
    /// <param name="properties"></param>
    /// <param name="mandatory"></param>
    /// <typeparam name="TMessage"></typeparam>
    Task PublishAsync<TMessage>(
        string routingKey,
        TMessage message,
        string exchange = "amq.direct",
        BasicProperties? properties = null,
        bool mandatory = false
    );

    /// <summary>
    /// 批量发送消息
    /// </summary>
    /// <param name="routingKey"></param>
    /// <param name="messages"></param>
    /// <param name="exchange"></param>
    /// <param name="properties"></param>
    /// <param name="mandatory"></param>
    /// <typeparam name="TMessage"></typeparam>
    Task PublishBatchAsync<TMessage>(
        string routingKey,
        IEnumerable<TMessage> messages,
        string exchange = "amq.direct",
        BasicProperties? properties = null,
        bool mandatory = false
    );
}
