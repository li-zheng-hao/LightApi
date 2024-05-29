using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LightApi.Infra.Autofac;
using LightApi.Infra.Http;
using LightApi.Infra.Options;
using LightApi.Infra.RabbitMQ;
using LightApi.Infra.Swagger;
using LightApi.Infra.Unify;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OOS.Core.Swagger;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtenions
{
    /// <summary>
    /// 配置登录用户上下文
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddUserContextSetup<T>(this IServiceCollection serviceCollection)
        where T : class, IUser
    {
        serviceCollection.AddScoped<IUser, T>();
        serviceCollection.AddScoped<T>();
        return serviceCollection;
    }
    /// <summary>
    /// LightApi框架基本配置
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="options">框架基本配置</param>
    /// <returns></returns>
    public static IServiceCollection AddLightApiSetup(this IServiceCollection serviceCollection,
        Action<InfrastructureOptions> options)
    {
        serviceCollection.Configure(options);

        serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureInfrastructureOption>();
        
        serviceCollection.AddUnifyResultProviderSetup(typeof(UnifyResult), typeof(UnifyResultProvider));

        return serviceCollection;
    }
    
    /// <summary>
    /// 配置Mapster
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddMapsterSetup(this IServiceCollection serviceCollection,params Assembly[] assemblies)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        // 全局忽略大小写
        TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
        typeAdapterConfig.Scan(assemblies);
        return serviceCollection;
    }
    
    /// <summary>
    /// 自定义统一返回包装类服务，不调用本方法则会使用默认的UnifyResult <see cref="UnifyResult"/>
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="unifyResultType">统一包装类，必须继承IUnifyResult接口</param>
    /// <param name="unifyResultProviderType">统一包装类提供服务，必须继承IUnifyResultProvider接口</param>
    /// <returns></returns>
    public static IServiceCollection AddUnifyResultProviderSetup(this IServiceCollection serviceCollection,
        Type? unifyResultType = null, Type? unifyResultProviderType = null)
    {
        if (unifyResultType is null)
            unifyResultType = typeof(UnifyResult);
        else
        {
            if (!unifyResultType.IsAssignableTo(typeof(IUnifyResult)))
                throw new ArgumentException("unifyResultType必须继承自IUnifyResult");
        }

        if (unifyResultProviderType != null)
        {
            if (!unifyResultProviderType.IsAssignableTo(typeof(IUnifyResultProvider)))
                throw new ArgumentException("unifyResultProviderType必须继承自IUnifyResultProvider");
        }
        else
        {
            unifyResultProviderType = typeof(UnifyResultProvider);
        }

        serviceCollection.Replace(new ServiceDescriptor(typeof(IUnifyResult), unifyResultType,
            ServiceLifetime.Transient));
        serviceCollection.Replace(new ServiceDescriptor(typeof(IUnifyResultProvider), unifyResultProviderType,
            ServiceLifetime.Singleton));

        return serviceCollection;
    }
    /// <summary>
    /// 注入swagger
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configure">自定义配置swagger</param>
    /// <param name="dllPrefixes">需要注册xml的dll前缀</param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerGenSetup(this ServiceCollection serviceCollection,Action<SwaggerGenOptions>? configure=null,params string[] dllPrefixes)
    {
        serviceCollection.AddSwaggerGenNewtonsoftSupport();

        serviceCollection.AddSwaggerGen(c =>
        {
            var assemblies= AppDomain.CurrentDomain.GetAssemblies().Where(it => dllPrefixes.Any(dp=> it.FullName?.StartsWith(dp)==true))
                .ToList();
           
            foreach (var xmlAssembly in assemblies)
            {
                var path = xmlAssembly.Location.Replace(".dll", ".xml");
                if (File.Exists(path))
                {
                    var doc = XDocument.Load(path);
                    c.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()), true);
                    c.SchemaFilter<DescribeEnumMembers>(doc);
                }
            }

            //允许上传文件
            c.OperationFilter<FileUploadFilter>();
            
            c.OperationFilter<ObsoleteOperationFilter>();

            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                if(docName == "v1")
                    return true;
                if (docName == apiDesc.GroupName) return true;
                return false;
            });

            configure?.Invoke(c);
        });

        return serviceCollection;
    }
    /// <summary>
    /// 注册RabbitMq相关服务
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqSetup(this IServiceCollection serviceCollection,IConfiguration configuration)
    {
        serviceCollection.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Section));

        serviceCollection.AddTransient<IRabbitMqPublisher, RabbitMqPublisher>();
        
        serviceCollection.AddSingleton<RabbitMqManager>(sp =>
        {
            var rabbitMqManager = new RabbitMqManager();
               
            var options = sp.GetService<IOptions<RabbitMqOptions>>();
               
            if (options?.Value.Configs == null)
                return rabbitMqManager;

            foreach (var config in options.Value.Configs)
            {
                var connection = new RabbitMqConnection(config);
                var rabbitMqPublisher = sp.GetService<IRabbitMqPublisher>();
                rabbitMqPublisher!.InitConnection(connection.Connection);
                rabbitMqManager.AddConnection(config.Key,connection);
                rabbitMqManager.AddPublisher(config.Key,rabbitMqPublisher);
            }
            return rabbitMqManager;
        });
        return serviceCollection;
    }
    
    /// <summary>
    /// 日志配置
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="systemName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddSerilogSetup(this WebApplicationBuilder builder,string systemName,Action<LoggerConfiguration>? configure)
    {
        string logTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}]  {Message:lj}{NewLine}{Exception}";
        var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("ENV", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("System", systemName)
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console(outputTemplate: logTemplate)
            .WriteTo.Async(a=>a.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,$"logs/{systemName}_.log")
                , rollingInterval: RollingInterval.Day, outputTemplate: logTemplate));
        
        configure?.Invoke(config);

        var logger = config.CreateLogger();
        Log.Logger = logger;
        builder.Host.UseSerilog(logger);
        return builder;
    }
    
    /// <summary>
    /// Autofac批量注册
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="dllPrefixes"></param>
    /// <returns></returns>
    public static IHostBuilder AddAutofacSetup(this IHostBuilder builder, params string[] dllPrefixes)
    {
        builder
            .UseServiceProviderFactory<
                ContainerBuilder>(new AutofacServiceProviderFactory())
            .ConfigureContainer(
                (Action<HostBuilderContext, ContainerBuilder>)((_, containerBuilder) =>
                    containerBuilder.RegisterModule(
                        new AutofacModuleRegister(dllPrefixes))));
        return builder;
    }
}