using System.Reflection;
using Autofac;
using LightApi.Core.FileProvider;
using LightApi.Core.Options;
using Masuit.Tools;
using Microsoft.Extensions.Configuration;
using PDM.Core.FileProvider;
using Module = Autofac.Module;

namespace LightApi.Core.Autofac;

/// <summary>
/// 
/// </summary>
public class AutofacModuleRegister : Module
{
    private readonly IConfiguration _configuration;
    private ContainerBuilder _builder;

    public AutofacModuleRegister(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        _builder = builder;

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

        AddFileProvider();
    }

    private void AddFileProvider()
    {
        var fileStorageOptions = _configuration.GetSection(FileStorageOptions.SectionName).Get<FileStorageOptions>();

        if (fileStorageOptions?.LocalStorages.IsNullOrEmpty() == false)
        {
            foreach (var localStorage in fileStorageOptions.LocalStorages!)
            {
                _builder.Register<IFileProvider>((it, b) =>
                {
                    var fileProvider = new LocalFileProvider
                    {
                        RootDir = localStorage.StorageRootDir
                    };
                    // 判断是否为相对路径
                    if (!Path.IsPathRooted(fileProvider.RootDir))
                    {
                        fileProvider.RootDir = Path.Combine(AppContext.BaseDirectory, fileProvider.RootDir);
                    }

                    fileProvider.FileNameGenerateStrategy = localStorage.FileNameGenerateStrategy;
                    return fileProvider;
                }).Named<IFileProvider>(localStorage.Key);
            }
        }

        if (fileStorageOptions?.MinIOStorages.IsNullOrEmpty() == false)
        {
            foreach (var minioStorage in fileStorageOptions.MinIOStorages!)
            {
                _builder.Register<IFileProvider>((it, b) =>
                {
                    var fileProvider = new MinioFileProvider(minioStorage);

                    return fileProvider;
                }).Named<IFileProvider>(minioStorage.Key);
            }
        }
      
    }
}