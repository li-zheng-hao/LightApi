using LightApi.Infra.DependencyInjections.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Infra.DependencyInjections;

public static class MiddlewareExtensions
{
    /// <summary>
    /// 注入之前配置过的所有中间件
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        App.Init(app);
        
        var setupOption = App.GetRequiredService<InfrastructureSetupOption>();

        app.UseResponseBodyReadMiddleware();
        
        foreach (var middlewareExtension in setupOption.MiddlewaresExtensions)
        {
            middlewareExtension.AddMiddleware(app);
        }

        return app;
    }
    /// <summary>
    /// 请求体可以多次读取
    /// </summary>
    /// <param name="app"></param>
    public static void UseResponseBodyReadMiddleware(this IApplicationBuilder app)
    {
        app.Use(async (httpContext, next) =>
        {
            httpContext.Request.EnableBuffering();
            await next.Invoke();
        });
    }
}