using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Infra.DependencyInjections.Core;

/// <summary>
/// 注入服务
/// </summary>
public interface IInfrastructureOptionsExtension
{
    /// <summary>
    /// Adds the services.
    /// </summary>
    /// <param name="services">Services.</param>
    void AddServices(IServiceCollection services);
}