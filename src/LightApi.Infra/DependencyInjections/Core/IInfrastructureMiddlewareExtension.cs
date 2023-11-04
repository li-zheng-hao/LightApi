using Microsoft.AspNetCore.Builder;

namespace LightApi.Infra.DependencyInjections.Core;

/// <summary>
/// 注入中间件
/// </summary>
public interface IInfrastructureMiddlewareExtension
{
    /// <summary>
    /// 注入中间件
    /// </summary>
    /// <param name="app"></param>
    void AddMiddleware(IApplicationBuilder app);
}