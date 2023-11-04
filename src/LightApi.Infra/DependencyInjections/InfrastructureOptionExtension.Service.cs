using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FB.Infrastructure;
using LightApi.Infra.DependencyInjections.Core;
using LightApi.Infra.Mapper;
using LightApi.Infra.Options;
using LightApi.Infra.Unify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;

namespace LightApi.Infra.DependencyInjections;

public static partial class InfrastructureOptionExtension
{
    /// <summary>
    /// 注入基础设施服务
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<InfrastructureSetupOption> configure)
    {
        serviceCollection.Configure<InfrastructureOptions>(configuration.GetSection(nameof(InfrastructureOptions)));

        serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureInfrastructureOption>();

        var options = new InfrastructureSetupOption();

        serviceCollection.AddSingleton(options);


        options.ServiceCollection = serviceCollection;
        options.Configuration = configuration;

        options.UseUnifyResultProvider(typeof(Unify.UnifyResult), typeof(DefaultUnifyResultProvider));

        configure(options);

        foreach (var serviceExtension in options.ServiceExtensions)
        {
            serviceExtension.AddServices(serviceCollection);
        }

        return serviceCollection;
    }


    /// <summary>
    /// 使用Mapster进行Dto映射
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static InfrastructureSetupOption UseMapster(this InfrastructureSetupOption option,
        params Assembly[] assemblies)
    {
        option.RegisterExtension(new MapsterOptionsExtension(assemblies));

        return option;
    }

    // /// <summary>
    // /// 引入Redis及分布式锁库
    // /// </summary>
    // /// <param name="option"></param>
    // /// <returns></returns>
    // public static InfrastructureSetupOption UseRedis(this InfrastructureSetupOption option, string connectionString)
    // {
    //     option.RegisterExtension(new RedisServiceCollectionExtension(connectionString));
    //
    //     return option;
    // }

    // /// <summary>
    // /// 配置默认Swagger服务
    // /// </summary>
    // /// <param name="option"></param>
    // /// <param name="xmlAssemblies">要抓取文档的程序集</param>
    // /// <returns></returns>
    // public static InfrastructureSetupOption UseSwagger(this InfrastructureSetupOption option,
    //     params Assembly[] xmlAssemblies)
    // {
    //     option.RegisterExtension(new SwaggerServiceCollectionExtension(xmlAssemblies));
    //
    //     return option;
    // }

    /// <summary>
    /// 手动配置默认的InfrastructureOptions，优先级比配置文件高
    /// </summary>
    /// <param name="option"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static InfrastructureSetupOption ConfigInfrastructureOptions(this InfrastructureSetupOption option,
        Action<InfrastructureOptions> action)
    {
        var serviceCollection = option.ServiceCollection;

        serviceCollection.Configure(action);

        return option;
    }

    // /// <summary>
    // /// 使用NewtonSoftJson作为默认的json序列化库
    // /// </summary>
    // /// <param name="option"></param>
    // /// <param name="action">MVC使用Json序列化的配置</param>
    // /// <param name="configure">JsonConvert的默认序列化配置</param>
    // /// <returns></returns>
    // public static InfrastructureSetupOption UseNewtonSoftJsonSupport(this InfrastructureSetupOption option,
    //     Action<MvcNewtonsoftJsonOptions> configure = null, Action<JsonSerializerSettings> settingsConfigure = null)
    // {
    //     var serviceCollection = option.ServiceCollection;
    //
    //     option.RegisterExtension(new NewtonSoftJsonServiceCollectionExtension(configure, settingsConfigure));
    //
    //     return option;
    // }

    /// <summary>
    /// 自定义统一返回包装类服务，不传参数则使用默认的UnifyResult
    /// <see cref="UnifyResult"/>
    /// <see cref="IUnifyResultProvider{T}"/>
    /// </summary>
    /// <param name="option"></param>
    /// <param name="unifyResultType">统一包装类，必须继承IUnifyResult接口</param>
    /// <param name="unifyResultProviderType">统一包装类提供服务，必须继承IUnifyResultProvider接口</param>
    /// <returns></returns>
    public static InfrastructureSetupOption UseUnifyResultProvider(this InfrastructureSetupOption option,
        Type? unifyResultType = null, Type? unifyResultProviderType = null)
    {
        var serviceCollection = option.ServiceCollection;

        if (unifyResultType is null)
            unifyResultType = typeof(Unify.UnifyResult);
        else
        {
            if (!unifyResultType.IsAssignableTo(typeof(IUnifyResult)))
                throw new ArgumentException("unifyResultType必须继承自IUnifyResult");
        }

        if (unifyResultProviderType != null)
        {
            if (!unifyResultProviderType.IsAssignableTo(typeof(IUnifyResultProvider)))
                throw new ArgumentException("unifyResultProviderType必须继承自IUnifyResultProvider");
        }
        else
        {
            unifyResultProviderType = typeof(DefaultUnifyResultProvider);
        }

        serviceCollection.Replace(new ServiceDescriptor(typeof(IUnifyResult), unifyResultType,
            ServiceLifetime.Transient));
        serviceCollection.Replace(new ServiceDescriptor(typeof(IUnifyResultProvider), unifyResultProviderType,
            ServiceLifetime.Singleton));

        return option;
    }
}