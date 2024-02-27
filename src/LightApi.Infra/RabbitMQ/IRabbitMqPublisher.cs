using System.Text;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using Serilog;

namespace LightApi.Infra.RabbitMQ;

public interface IRabbitMqPublisher
{
    void InitConnection(IConnection connection);
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="routingKey"></param>
    /// <param name="message"></param>
    /// <param name="exchange"></param>
    /// <param name="properties"></param>
    /// <param name="mandatory"></param>
    /// <typeparam name="TMessage"></typeparam>
    void Publish<TMessage>(
        string routingKey
        , TMessage message
        , string exchange = "amq.direct"
        , IBasicProperties? properties = null
        , bool mandatory = false
    );

    IBasicProperties? CreateBasicProperties();

}