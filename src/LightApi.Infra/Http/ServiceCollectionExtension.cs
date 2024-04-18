using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Infra.Http;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 配置登录用户上下文
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddUserContextSetup<T>(this IServiceCollection serviceCollection)
        where T : class, IUser
    {
        serviceCollection.AddScoped<IUser, T>();
        serviceCollection.AddScoped<T>();
        return serviceCollection;
    }
}