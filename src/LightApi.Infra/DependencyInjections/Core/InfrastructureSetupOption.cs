using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Infra.DependencyInjections.Core;

public class InfrastructureSetupOption
{

    /// <summary>
    /// Initializes a new instance of the <see cref="IInfrastructureOptionsExtension"/> class.
    /// </summary>
    public InfrastructureSetupOption()
    {
        ServiceExtensions = new List<IInfrastructureOptionsExtension>();
        MiddlewaresExtensions = new List<IInfrastructureMiddlewareExtension>();
    }

    /// <summary>
    /// 基础框架中注册的服务
    /// </summary>
    /// <value>The extensions.</value>
    internal IList<IInfrastructureOptionsExtension> ServiceExtensions { get; }
    /// <summary>
    /// 基础框架中使用的中间件
    /// </summary>
    internal IList<IInfrastructureMiddlewareExtension> MiddlewaresExtensions { get; }
    
    internal IConfiguration Configuration { get; set; }

    internal IServiceCollection ServiceCollection { get; set; }

    /// <summary>
    /// Registers the extension.
    /// </summary>
    /// <param name="extension">Extension.</param>
    public void RegisterExtension(IInfrastructureOptionsExtension extension)
    {
        ArgumentNullException.ThrowIfNull(extension);
        ServiceExtensions.Add(extension);
    }
    /// <summary>
    /// Registers the extension.
    /// </summary>
    /// <param name="extension">Extension.</param>
    public void RegisterMiddleware(IInfrastructureMiddlewareExtension middlewareExtension)
    {
        ArgumentNullException.ThrowIfNull(middlewareExtension);
        MiddlewaresExtensions.Add(middlewareExtension);
    }
}