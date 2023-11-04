using LightApi.Infra.DependencyInjections.Core;

namespace LightApi.Infra.DependencyInjections;

public static partial class InfrastructureOptionExtension
{
   public static InfrastructureSetupOption UseDemoMiddleware(this InfrastructureSetupOption setupOption)
   {
      // setupOption.RegisterMiddleware(new DemoMiddlewareExtension());

      return setupOption;
   }
}