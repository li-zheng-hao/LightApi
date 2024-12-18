using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace LightApi.Infra.RabbitMQ
{
    public abstract class BaseRabbitMqConsumer : IHostedService
    {
        protected RabbitMqManager _rabbitMqManager;
        protected IConnection? _connection;
        protected IChannel? _channel;

        protected BaseRabbitMqConsumer(RabbitMqManager rabbitMqManager)
        {
            _rabbitMqManager = rabbitMqManager;
        }

        /// <summary>
        /// 通过RabbitMqManager初始化连接
        /// </summary>
        /// <returns></returns>
        public virtual async Task InitChannel()
        {
            this._connection = _rabbitMqManager.GetConnection().Connection;
            this._channel = await _connection.CreateChannelAsync();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitChannel();
            Register();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            DeRegister();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 注册消费者
        /// </summary>
        protected virtual async Task Register()
        {
            //获取交换机配置
            var exchange = GetExchangeConfig();

            //获取routingKeys
            var routingKeys = GetRoutingKeys();

            //获取队列配置
            var queue = GetQueueConfig();

            //声明死信交换与队列
            await this.RegisterDeadExchangeAsync(
                exchange.DeadExchangeName,
                queue.DeadQueueName,
                routingKeys,
                queue.Durable
            );

            //声明交换机
            if (exchange.AutoCreate)
                await _channel!.ExchangeDeclareAsync(
                    exchange.Name,
                    type: exchange.Type.ToString().ToLower()
                );

            //声明队列
            await _channel!.QueueDeclareAsync(
                queue: queue.Name,
                durable: queue.Durable,
                exclusive: queue.Exclusive,
                autoDelete: queue.AutoDelete,
                arguments: queue.Arguments
            );

            //将队列与交换机进行绑定
            if (routingKeys == null || routingKeys.Length == 0)
            {
                await _channel.QueueBindAsync(
                    queue: queue.Name,
                    exchange: exchange.Name,
                    routingKey: string.Empty
                );
            }
            else
            {
                foreach (var key in routingKeys)
                {
                    await _channel.QueueBindAsync(
                        queue: queue.Name,
                        exchange: exchange.Name,
                        routingKey: key
                    );
                }
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);

            //关闭自动确认,开启手动确认后需要配置这些
            if (!queue.AutoAck)
            {
                await _channel.BasicQosAsync(
                    prefetchSize: 0,
                    prefetchCount: queue.PrefetchCount,
                    global: queue.Global
                );
                await _channel.BasicConsumeAsync(
                    queue: queue.Name,
                    consumer: consumer,
                    autoAck: queue.AutoAck
                );
            }

            consumer.ReceivedAsync += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                bool result = await ProcessAsync(ea.Exchange, ea.RoutingKey, message);

                Log.Debug($"result:{result},message:{message}");

                //关闭自动确认,开启手动确认后需要依据处理结果选择返回确认信息。
                if (!queue.AutoAck)
                    if (result)
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: queue.AckMultiple);
                    else
                        await _channel.BasicRejectAsync(
                            ea.DeliveryTag,
                            requeue: queue.RejectRequeue
                        );
            };
        }

        /// <summary>
        /// 注销/关闭连接
        /// </summary>
        protected virtual async Task DeRegister()
        {
            await _channel!.CloseAsync();
            _channel!.Dispose();
            await _connection!.CloseAsync();
            _connection!.Dispose();
        }

        /// <summary>
        /// 获取交互机列配置
        /// </summary>
        /// <returns></returns>
        protected abstract ExchageConfig GetExchangeConfig();

        /// <summary>
        /// 获取路由keys
        /// </summary>
        /// <returns></returns>
        protected abstract string[] GetRoutingKeys();

        /// <summary>
        /// 获取队列配置
        /// </summary>
        /// <returns></returns>
        protected abstract QueueConfig GetQueueConfig();

        /// <summary>
        /// 获取队列公共配置
        /// </summary>
        /// <returns></returns>
        protected QueueConfig GetCommonQueueConfig()
        {
            return new QueueConfig()
            {
                Name = string.Empty,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = true,
                AutoAck = false,
                AckMultiple = false,
                PrefetchCount = 1,
                RejectRequeue = false,
                Arguments = null
            };
        }

        /// <summary>
        /// 处理消息的方法
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        protected abstract Task<bool> ProcessAsync(
            string exchange,
            string routingKey,
            string message
        );

        /// <summary>
        /// 声明死信交换与队列
        /// </summary>
        protected virtual async Task RegisterDeadExchangeAsync(
            string deadExchangeName,
            string deadQueueName,
            string[] routingKeys,
            bool durable
        )
        {
            if (!string.IsNullOrWhiteSpace(deadExchangeName))
            {
                await _channel!.ExchangeDeclareAsync(
                    deadExchangeName,
                    ExchangeType.Direct.ToString().ToLower()
                );
                await _channel!.QueueDeclareAsync(
                    queue: deadQueueName,
                    durable: durable,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                foreach (var key in routingKeys)
                {
                    await _channel.QueueBindAsync(
                        queue: deadQueueName,
                        exchange: deadExchangeName,
                        routingKey: key
                    );
                }
            }
        }
    }
}
