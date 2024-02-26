using LightApi.Infra.DependencyInjections.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightApi.Infra.RabbitMQ;

public class RabbitMqManagerExtension:IInfrastructureOptionsExtension
{
    private readonly IConfiguration _configuration;

    public RabbitMqManagerExtension(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void AddServices(IServiceCollection services)
    {
        services.Configure<RabbitMqOptions>(_configuration.GetSection(RabbitMqOptions.Section));

        services.AddSingleton<RabbitMqManager>(sp =>
        {
            var rabbitMqManager = new RabbitMqManager();
               
            var options = sp.GetService<IOptions<RabbitMqOptions>>();
               
            if (options?.Value.Configs == null)
                return rabbitMqManager;
               
            foreach (var config in options.Value.Configs)
                rabbitMqManager.AddConnection(config.Key,new RabbitMqConnection(config));
            return rabbitMqManager;
        });
    }
}