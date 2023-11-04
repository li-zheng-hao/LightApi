using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Infra;

public class App
{
    public static void Init(WebApplication app)
    {
        Configuration = app.Configuration;
        HostEnvironment = app.Environment;
        ServiceProvider = app.Services;
    }
    public static IConfiguration Configuration { get; set; }

    public static IWebHostEnvironment HostEnvironment { get; set; }

    /// <summary>
    /// 根服务提供者，只允许获取单例，其它生命周期需要使用 CreateScope() 创建新的作用域，否则有内存泄露
    /// </summary>
    public static IServiceProvider ServiceProvider { get; set; }

    public static object GetService(Type serviceType)
    {
        return ServiceProvider.GetService(serviceType);
    }

    public static T GetService<T>()
    {
        return ServiceProvider.GetService<T>();
    }

    public static IEnumerable<T> GetServices<T>()
    {
        return ServiceProvider.GetServices<T>();
    }

    public static object GetRequiredService(Type serviceType)
    {
        return ServiceProvider.GetRequiredService(serviceType);
    }

    public static T GetRequiredService<T>()
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    public static IEnumerable<object> GetServices(Type serviceType)
    {
        return ServiceProvider.GetServices(serviceType);
    }

    public static T GetConfig<T>(string key)
    {
        return Configuration.GetSection(key).Get<T>();
    }
}