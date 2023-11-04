using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection ReplaceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Replace(ServiceDescriptor.Singleton(configuration));
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            var hostBuilderContext = services.GetSingletonInstanceOrNull<HostBuilderContext>();
            if (hostBuilderContext?.Configuration is not null)
            {
                var instance = hostBuilderContext.Configuration as IConfigurationRoot;
                if (instance is not null)
                    return instance;
            }

            return services.GetSingletonInstance<IConfiguration>();
        }
    }
}