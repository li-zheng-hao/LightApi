using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightApi.Infra.Unify;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 自定义统一返回包装类服务，不调用本方法则会使用默认的UnifyResult <see cref="UnifyResult"/>
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="unifyResultType">统一包装类，必须继承IUnifyResult接口</param>
    /// <param name="unifyResultProviderType">统一包装类提供服务，必须继承IUnifyResultProvider接口</param>
    /// <returns></returns>
    public static IServiceCollection AddUnifyResultProviderSetup(this IServiceCollection serviceCollection,
        Type? unifyResultType = null, Type? unifyResultProviderType = null)
    {
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

        return serviceCollection;
    }

}