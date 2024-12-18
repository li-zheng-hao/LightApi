namespace LightApi.Infra.RabbitMQ;

public class RabbitMqManager
{
    public Dictionary<string, IRabbitMqConnection> Connections { get; set; } = new();
    public Dictionary<string, IRabbitMqPublisher> Publishers { get; set; } = new();

    public void AddConnection(string key, IRabbitMqConnection connection)
    {
        Connections.TryAdd(key, connection);
    }

    public void AddPublisher(string key, IRabbitMqPublisher publisher)
    {
        Publishers.TryAdd(key, publisher);
    }

    public IRabbitMqConnection GetConnection(string key = "default")
    {
        return Connections.TryGetValue(key!, out var connection)
            ? connection
            : throw new Exception($"未找到key为{key}的RabbitMQ连接");
    }

    public IRabbitMqPublisher GetPublisher(string key = "default")
    {
        return Publishers.TryGetValue(key!, out var publisher)
            ? publisher
            : throw new Exception($"未找到key为{key}的RabbitMQ发送者");
    }
}
