using System.Reflection;
using LightApi.Infra.DependencyInjections.Core;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Infra.Mapper;

public class MapsterOptionsExtension:IInfrastructureOptionsExtension
{
    private readonly Assembly[] _assemblies;

    public MapsterOptionsExtension(Assembly[] assemblies)
    {
        _assemblies = assemblies;
    }
    public void AddServices(IServiceCollection services)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        // 全局忽略大小写
        TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
        Assembly applicationAssembly = typeof(BaseDto<,>).Assembly;
        typeAdapterConfig.Scan(_assemblies);
    }
}