using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LightApi.Infra;

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

        app.UseResponseBodyReadMiddleware();

        return app;
    }

    /// <summary>
    /// 请求体可以多次读取
    /// </summary>
    /// <param name="app"></param>
    public static void UseResponseBodyReadMiddleware(this IApplicationBuilder app)
    {
        app.Use(
            async (httpContext, next) =>
            {
                httpContext.Request.EnableBuffering();
                await next.Invoke();
            }
        );
    }
}
