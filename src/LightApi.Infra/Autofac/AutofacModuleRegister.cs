using System.Diagnostics;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace LightApi.Infra.Autofac;

/// <summary>
/// autofac模块注册
/// </summary>
public class AutofacModuleRegister : Module
{
    private readonly string[] _dllPrefixes;
    private readonly string _basePath;

    /// <summary>
    ///
    /// </summary>
    /// <param name="dllPrefixes">需要注册的dll前缀</param>
    /// <param name="basePath">dll所在目录，默认为当前应用程序目录</param>
    public AutofacModuleRegister(string[] dllPrefixes, string? basePath = null)
    {
        _dllPrefixes = dllPrefixes;
        _basePath = basePath ?? AppContext.BaseDirectory;
    }

    protected override void Load(ContainerBuilder builder)
    {
        var assemblies = LoadAssembliesFromDisk();

        foreach (var assembly in assemblies)
        {
            RegisterAssemblyTypes(builder, assembly);
        }
    }

    private List<Assembly> LoadAssembliesFromDisk()
    {
        var assemblies = new List<Assembly>();

        if (!Directory.Exists(_basePath))
        {
            return assemblies;
        }

        // 获取所有匹配前缀的dll文件
        var dllFiles = Directory.GetFiles(_basePath, "*.dll")
            .Where(file => _dllPrefixes.Any(prefix =>
                Path.GetFileNameWithoutExtension(file)?.StartsWith(prefix) == true))
            .ToList();

        foreach (var dllFile in dllFiles)
        {
            try
            {
                // 尝试加载程序集
                var assembly = Assembly.LoadFrom(dllFile);
                assemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"加载程序集失败: {dllFile}, 错误: {ex.Message}");
            }
        }

        return assemblies;
    }

    private void RegisterAssemblyTypes(ContainerBuilder builder, Assembly assembly)
    {
        var exportedTypes = assembly.GetExportedTypes().ToArray();

        var transientTypes = new List<Type>();
        var scopedTypes = new List<Type>();
        var singletonTypes = new List<Type>();

        foreach (var type in exportedTypes)
        {
            if (type.IsClass && !type.IsAbstract)
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

        // 注册Transient类型
        if (transientTypes.Any())
        {
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
        }

        // 注册Scoped类型
        if (scopedTypes.Any())
        {
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
        }

        // 注册Singleton类型
        if (singletonTypes.Any())
        {
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
    }
}
