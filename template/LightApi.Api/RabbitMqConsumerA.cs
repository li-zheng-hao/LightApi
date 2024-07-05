﻿using LightApi.Infra;
using LightApi.Infra.RabbitMQ;
using Serilog;

namespace LightApi.Api;

public class RabbitMqConsumerA:BaseRabbitMqConsumer
{

    public override void InitChannel()
    {
        this._connection = App.GetService<RabbitMqManager>()!.GetConnection().Connection;
        this._channel = this._connection.CreateModel();
    }

    protected override ExchageConfig GetExchangeConfig()
    {
        return new ExchageConfig()
        {
            Name = "amq.direct",
            Type = ExchangeType.Direct,
        };
    }

    protected override string[] GetRoutingKeys()
    {
        return new[] { "test_a" };
    }

    protected override QueueConfig GetQueueConfig()
    {
        return new QueueConfig()
        {
            AckMultiple = false,
            AutoAck = false,
            Name = "test_a",
            PrefetchCount = 1,
            RejectRequeue = true,
        };
    }

    protected override async Task<bool> Process(string exchange, string routingKey, string message)
    {
        Log.Information(message);
        await Task.Delay(1000);
        return true;
    }

    public RabbitMqConsumerA(RabbitMqManager rabbitMqManager) : base(rabbitMqManager)
    {
    }
}