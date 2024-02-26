using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using Serilog;

namespace LightApi.Infra.RabbitMQ
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly IModel? _channel;

        public RabbitMqPublisher(IRabbitMqConnection rabbitMqConnection)
        {
            _channel = rabbitMqConnection.Connection.CreateModel();
        }

        public virtual void BasicPublish<TMessage>(
             string routingKey
            , TMessage message
             , string exchange="amq.direct"
            , IBasicProperties? properties = null
            , bool mandatory = false
        )
        {
            Policy.Handle<Exception>()
                  .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(1), (ex, time, retryCount, content) =>
                  {
                      Log.Error(ex, string.Format("{0}:{1}", retryCount, ex.Message));
                  })
                  .Execute(() =>
                  {
                      var content = message as string ?? JsonConvert.SerializeObject(message);

                      var body = Encoding.UTF8.GetBytes(content);
                      //当mandatory标志位设置为true时，如果exchange根据自身类型和消息routingKey无法找到一个合适的queue存储消息
                      //那么broker会调用basic.return方法将消息返还给生产者;
                      //当mandatory设置为false时，出现上述情况broker会直接将消息丢弃
                      _channel?.BasicPublish(exchange, routingKey, mandatory, basicProperties: properties, body);

                      // 开启发布消息确认模式
                      // _channel.ConfirmSelect();
                      // 消息是否到达服务器
                      // bool publishStatus = _channel.WaitForConfirms();
                  });
        }

        public virtual IBasicProperties? CreateBasicProperties()
        {
            return _channel?.CreateBasicProperties();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _channel?.Dispose();
            }
        }
    }
}