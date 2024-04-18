using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightApi.Infra.RabbitMQ;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 注册RabbitMq相关服务
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqSetup(this IServiceCollection serviceCollection,IConfiguration configuration)
    {
        serviceCollection.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Section));

        serviceCollection.AddTransient<IRabbitMqPublisher, RabbitMqPublisher>();
        
        serviceCollection.AddSingleton<RabbitMqManager>(sp =>
        {
            var rabbitMqManager = new RabbitMqManager();
               
            var options = sp.GetService<IOptions<RabbitMqOptions>>();
               
            if (options?.Value.Configs == null)
                return rabbitMqManager;

            foreach (var config in options.Value.Configs)
            {
                var connection = new RabbitMqConnection(config);
                var rabbitMqPublisher = sp.GetService<IRabbitMqPublisher>();
                rabbitMqPublisher!.InitConnection(connection.Connection);
                rabbitMqManager.AddConnection(config.Key,connection);
                rabbitMqManager.AddPublisher(config.Key,rabbitMqPublisher);
            }
            return rabbitMqManager;
        });
        return serviceCollection;
    }
}