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

        assemblies.ForEach(it =>
        {
            var transient = it.GetExportedTypes()
                .Where(type => type.IsClass && type.IsAssignableTo(typeof(ITransientDependency)))
                .ToArray();

            builder
                .RegisterTypes(transient)
                .AsSelf()
                .InstancePerDependency()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder
                .RegisterTypes(transient)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            var scoped = it.GetExportedTypes()
                .Where(type => type.IsClass && type.IsAssignableTo(typeof(IScopedDependency)))
                .ToArray();
            builder
                .RegisterTypes(scoped)
                .AsSelf()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder
                .RegisterTypes(scoped)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            var singleton = it.GetExportedTypes()
                .Where(type => type.IsClass && type.IsAssignableTo(typeof(ISingletonDependency)))
                .ToArray();
            builder
                .RegisterTypes(singleton)
                .AsSelf()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder
                .RegisterTypes(singleton)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        });
    }
}
