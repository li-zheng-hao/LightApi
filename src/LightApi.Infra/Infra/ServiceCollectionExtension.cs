using LightApi.Infra.Options;
using LightApi.Infra.Unify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightApi.Infra.Infra;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// LightApi框架基本配置
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="options">框架基本配置</param>
    /// <returns></returns>
    public static IServiceCollection AddLightApiSetup(this IServiceCollection serviceCollection,
        Action<InfrastructureOptions> options)
    {
        serviceCollection.Configure(options);

        serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureInfrastructureOption>();
        
        serviceCollection.AddUnifyResultProviderSetup(typeof(Unify.UnifyResult), typeof(DefaultUnifyResultProvider));

        return serviceCollection;
    }
 
}