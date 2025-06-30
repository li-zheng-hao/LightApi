using System.Diagnostics;
using Autofac;

namespace LightApi.Infra.Autofac;

/// <summary>
/// autofac模块注册
/// </summary>
public class AutofacModuleRegister : Module
{
    private readonly string[] _dllPrefixes;

    /// <summary>
    ///
    /// </summary>
    /// <param name="dllPrefixes">需要注册的dll前缀</param>
    public AutofacModuleRegister(params string[] dllPrefixes)
    {
        _dllPrefixes = dllPrefixes;
    }

    protected override void Load(ContainerBuilder builder)
    {
        var assemblies = AppDomain
            .CurrentDomain.GetAssemblies()
            .Where(it => _dllPrefixes.Any(df => it.FullName?.StartsWith(df) == true))
            .ToList();

        Parallel.ForEach(
            assemblies,
            it =>
            {
                var exportedTypes = it.GetExportedTypes().ToArray();

                var transientTypes = new List<Type>();
                var scopedTypes = new List<Type>();
                var singletonTypes = new List<Type>();

                foreach (var type in exportedTypes)
                {
                    if (type.IsClass)
                    {
                        if (type.IsAssignableTo(typeof(ITransientDependency)))
                        {
                            transientTypes.Add(type);
                        }
                        if (type.IsAssignableTo(typeof(IScopedDependency)))
                        {
                            scopedTypes.Add(type);
                        }
                        if (type.IsAssignableTo(typeof(ISingletonDependency)))
                        {
                            singletonTypes.Add(type);
                        }
                    }
                }

                builder
                    .RegisterTypes(transientTypes.ToArray())
                    .AsSelf()
                    .InstancePerDependency()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

                builder
                    .RegisterTypes(transientTypes.ToArray())
                    .AsImplementedInterfaces()
                    .InstancePerDependency()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

                builder
                    .RegisterTypes(scopedTypes.ToArray())
                    .AsSelf()
                    .InstancePerLifetimeScope()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

                builder
                    .RegisterTypes(scopedTypes.ToArray())
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

                builder
                    .RegisterTypes(singletonTypes.ToArray())
                    .AsSelf()
                    .SingleInstance()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

                builder
                    .RegisterTypes(singletonTypes.ToArray())
                    .AsImplementedInterfaces()
                    .SingleInstance()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            }
        );
    }
}
