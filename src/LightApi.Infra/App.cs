using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LightApi.Infra;

public class App
{
    public static void Init(WebApplication app)
    {
        Configuration = app.Configuration;
        HostEnvironment = app.Environment;
        ServiceProvider = app.Services;
        AutofacContainer=app.Services.GetAutofacRoot();
        AppLifeTime=app.Lifetime;
    }

    /// <summary>
    /// 程序生命周期
    /// </summary>
    public static IHostApplicationLifetime AppLifeTime { get; set; }

    public static ILifetimeScope AutofacContainer { get; set; }
    
    public static IConfiguration Configuration { get; set; }

    public static IWebHostEnvironment HostEnvironment { get; set; }

    /// <summary>
    /// 根服务提供者，只允许获取单例，其它生命周期需要使用 CreateScope() 创建新的作用域，否则有内存泄露
    /// </summary>
    public static IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static object? GetService(Type serviceType)
    {
        return ServiceProvider.GetService(serviceType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetService<T>()
    {
        return ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// 获取命名服务
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetNamedService<T>(string name) where T : class
    {
        return AutofacContainer.ResolveNamed(serviceName:name,serviceType:typeof(T)) as T;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> GetServices<T>()
    {
        return ServiceProvider.GetServices<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static object GetRequiredService(Type serviceType)
    {
        return ServiceProvider.GetRequiredService(serviceType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRequiredService<T>()
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static IEnumerable<object?> GetServices(Type serviceType)
    {
        return ServiceProvider.GetServices(serviceType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetConfig<T>(string key)
    {
        return Configuration.GetSection(key).Get<T>();
    }
}