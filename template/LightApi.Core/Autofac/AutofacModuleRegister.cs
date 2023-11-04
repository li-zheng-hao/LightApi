using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace LightApi.Core.Autofac;

/// <summary>
/// 
/// </summary>
public class AutofacModuleRegister : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var basePath = AppContext.BaseDirectory;
        
        var servicesDllFile = Path.Combine(basePath, "LightApi.Service.dll");
        
        var repositoryDllFile = Path.Combine(basePath, "LightApi.Repository.dll");

        if (!(File.Exists(servicesDllFile) && File.Exists(repositoryDllFile)))
        {
            var msg = "Repository.dll和Services.dll 丢失，因为项目解耦了，所以需要先F6编译，再F5运行，请检查 bin 文件夹，并拷贝。";
            throw new ArgumentException(msg);
        }

        // 获取 Service.dll 程序集服务，并注册
        var assemblysServices = Assembly.LoadFrom(servicesDllFile);
        var services = assemblysServices.GetTypes().Where(it => it.IsClass && it.FullName!.EndsWith("Service"))
            .ToArray();
        //支持属性注入依赖重复
        builder.RegisterTypes(services).AsSelf().InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        //支持属性注入依赖重复
        builder.RegisterTypes(services).AsImplementedInterfaces().InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

        // 获取 Repository.dll 程序集服务，并注册
        var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
        var repos = assemblysRepository.GetTypes().Where(it => it.IsClass && it.FullName!.EndsWith("Repository"))
            .ToArray();

        //支持属性注入依赖循环
        builder.RegisterTypes(repos).AsSelf().InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

        var coreAssemblies = new Assembly[] { Assembly.Load("LightApi.Core") };

        coreAssemblies.ToList().ForEach(it =>
        {
            var transient = it.GetExportedTypes()
                .Where(it => it.IsClass && it.IsAssignableTo(typeof(ITransientDependency))).ToArray();
            builder.RegisterTypes(transient).AsSelf().InstancePerDependency()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            var scoped = it.GetExportedTypes()
                .Where(it => it.IsClass && it.IsAssignableTo(typeof(IScopedDependency))).ToArray();
            builder.RegisterTypes(scoped).AsSelf().InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            var singleton = it.GetExportedTypes()
                .Where(it => it.IsClass && it.IsAssignableTo(typeof(ISingletonDependency))).ToArray();
            builder.RegisterTypes(singleton).AsSelf().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        });
    }
}