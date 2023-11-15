using LightApi.Infra.DependencyInjections.Core;
using Microsoft.AspNetCore.Builder;
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

        foreach (var middlewareExtension in setupOption.MiddlewaresExtensions)
        {
            middlewareExtension.AddMiddleware(app);
        }

        return app;
    }
}