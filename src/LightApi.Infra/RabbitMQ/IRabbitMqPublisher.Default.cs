using System.Text;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using Serilog;

namespace LightApi.Infra.RabbitMQ
{
    public class RabbitMqPublisher : IAsyncDisposable, IRabbitMqPublisher
    {
        protected IChannel? _channel;

        public async Task InitConnection(IConnection connection)
        {
            _channel = await connection.CreateChannelAsync();
        }

        public virtual async Task PublishAsync<TMessage>(
            string routingKey,
            TMessage message,
            string exchange = "amq.direct",
            BasicProperties? properties = null,
            bool mandatory = false
        )
        {
            if (_channel == null)
                throw new ArgumentNullException(nameof(_channel), "channel is null");

            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(1),
                    (ex, time, retryCount, content) =>
                    {
                        Log.Error(ex, $"Rabbitmq发送消息失败：{retryCount}:{ex.Message}");
                    }
                )
                .ExecuteAsync(async () =>
                {
                    var content = message as string ?? JsonConvert.SerializeObject(message);
                    Log.Debug($"Rabbitmq发送消息：{exchange}:{routingKey}:{content}");
                    var body = Encoding.UTF8.GetBytes(content);
                    //当mandatory标志位设置为true时，如果exchange根据自身类型和消息routingKey无法找到一个合适的queue存储消息
                    //那么broker会调用basic.return方法将消息返还给生产者;
                    //当mandatory设置为false时，出现上述情况broker会直接将消息丢弃
                    // 会自动确认发布，被nack或者返回会抛出异常
                    if (properties == null)
                        await _channel!
                            .BasicPublishAsync(exchange, routingKey, mandatory, body)
                            .ConfigureAwait(false);
                    else
                        await _channel!
                            .BasicPublishAsync(exchange, routingKey, mandatory, properties, body)
                            .ConfigureAwait(false);
                });
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
        }
    }
}
