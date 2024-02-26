using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace LightApi.Infra.RabbitMQ
{
    public interface IRabbitMqConnection
    {
        IConnection Connection { get; }
    }

    internal sealed class RabbitMqConnection : IRabbitMqConnection
    {
        public IConnection Connection { get; private set; } = default!;
        public RabbitMqConnection(RabbitMqConfig options)
        {

            var factory = new ConnectionFactory()
            {
                HostName = options.HostName,
                VirtualHost = options.VirtualHost,
                UserName = options.UserName,
                Password = options.Password,
                Port = options.Port,
                //Rabbitmq集群必需加这两个参数
                AutomaticRecoveryEnabled = true,
                //TopologyRecoveryEnabled=true
            };

            Policy.Handle<SocketException>()
                  .Or<BrokerUnreachableException>()
                  .WaitAndRetry(2, retryAttempt => TimeSpan.FromSeconds(1), (ex, time, retryCount, content) =>
                  {
                      if (2 == retryCount)
                          throw ex;
                      Log.Error(ex, string.Format("{0}:{1}", retryCount, ex.Message));
                  })
                  .Execute(() =>
                  {
                      Connection = factory.CreateConnection();
                  });
        }
    }
}