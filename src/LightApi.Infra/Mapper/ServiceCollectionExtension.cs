using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Infra.Mapper;

public static class ServiceCollectionExtension
{
    
    /// <summary>
    /// 配置Mapster
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddMapsterSetup(this IServiceCollection serviceCollection,params Assembly[] assemblies)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        // 全局忽略大小写
        TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
        typeAdapterConfig.Scan(assemblies);
        return serviceCollection;
    }
}