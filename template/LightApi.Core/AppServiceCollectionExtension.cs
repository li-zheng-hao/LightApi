using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using LightApi.Core.Aop;
using LightApi.Core.Authorization;
using LightApi.Core.Autofac;
using LightApi.Core.Converter;
using LightApi.Core.FileProvider;
using LightApi.Core.Swagger;
using LightApi.Domain;
using LightApi.EFCore;
using LightApi.EFCore.Config;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Interceptors;
using LightApi.EFCore.Repository;
using LightApi.Infra.DependencyInjections;
using LightApi.Infra.Extension;
using Masuit.Tools.Systems;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nacos.AspNetCore.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus.SystemMetrics;
using Prometheus.SystemMetrics.Collectors;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

namespace LightApi.Core;

public static class AppServiceCollectionExtension
{
    public static IServiceCollection AddMonitorSetup(this IServiceCollection serviceCollection)
    {
        if (Equals(System.Globalization.CultureInfo.CurrentCulture,
                System.Globalization.CultureInfo.InvariantCulture) &&
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            serviceCollection.AddSystemMetrics(registerDefaultCollectors: false);
            serviceCollection.AddSystemMetricCollector<DiskCollector>();
            serviceCollection.AddSystemMetricCollector<WindowsMemoryCollector>();
            serviceCollection.AddSystemMetricCollector<LoadAverageCollector>();
            serviceCollection.AddSystemMetricCollector<NetworkCollector>();
        }
        else
        {
            serviceCollection.AddSystemMetrics();
        }

        return serviceCollection;
    }

    public static IServiceCollection AddInfra(this IServiceCollection serviceCollection, WebApplicationBuilder builder)
    {
        serviceCollection.AddInfrastructure(builder.Configuration, configure =>
        {
            configure.ConfigInfrastructureOptions(it =>
            {
                it.EnableGlobalModelValidator = true;
                it.EnableGlobalUnifyResult = true;
                it.EnableGlobalExceptionFilter = true;
                it.DefaultModelValidateErrorMessage = BusinessErrorCode.Code400.GetDescription();
                it.DefaultModelValidateErrorBusinessCode = (int)BusinessErrorCode.Code400;
                it.DefaultModelValidateErrorHttpStatusCode = HttpStatusCode.OK;
                it.UseFirstModelValidateErrorMessage = true;
            });
            configure.UseUserContext<UserContext>();
            configure.UseMapster(Assembly.Load("LightApi.Service"));
        });

        serviceCollection.AddScoped<UserContext>();

      

        return serviceCollection;
    }

    public static IServiceCollection AddFileProviderSetup(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IFileProvider, LocalFileProvider>(sp =>
        {
            var configuration = sp.GetService<IConfiguration>();
            var rootDir = configuration!["Other:LocalStorageDir"];
            var fileProvider = new LocalFileProvider()
            {
                RootDir = rootDir ?? Path.Combine(AppContext.BaseDirectory, "FileData"),
                FileNameGenerateStrategy = FileNameGenerateStrategy.TimeStampWithDayPrefix
            };
            Directory.CreateDirectory(fileProvider.RootDir);
            return fileProvider;
        });

        return serviceCollection;
    }
    public static IServiceCollection AddCorsSetup(this IServiceCollection serviceCollection)
    {
        // 此处根据自己的需要配置可通过的域名或ip
        serviceCollection.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.SetIsOriginAllowed(it => true);
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowCredentials();
                });
        });

        return serviceCollection;
    }

    public static IServiceCollection AddAuthorizeSetup(this IServiceCollection serviceCollection)
    {
        // 全局启动授权校验
        // serviceCollection.AddAuthorization(options =>
        // {
        //     options.FallbackPolicy = new AuthorizationPolicyBuilder()
        //         .RequireAuthenticatedUser()
        //         .Build();
        // });
        // serviceCollection.AddAuthentication(option =>
        //     {
        //         option.DefaultScheme = CustomAuthorizationSchemes.HybridSchemeName;
        //     })
        //     .AddScheme<CustomHybridAuthSchemeOptions, CustomHybridAuthHandler>(CustomAuthorizationSchemes.HybridSchemeName,
        //         i => { })
        //     .AddScheme<CustomJwtAuthSchemeOptions, CustomJwtAuthHandler>(CustomAuthorizationSchemes.JwtSchemeName, i => { })
        //     .AddScheme<CustomApiAuthSchemeOptions, CustomApiAuthHandler>(CustomAuthorizationSchemes.ApiSchemeName, i => { });
        return serviceCollection;
    }

    /// <summary>
    /// 分布式锁
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddDistributedLockSetup(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDistributedLockProvider>(_ =>
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "locks");
            Directory.CreateDirectory(filePath);

            return new FileDistributedSynchronizationProvider(new DirectoryInfo(filePath));
        });

        return serviceCollection;
    }
    /// <summary>
    /// 加载锁
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddEasyCachingSetup(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddEasyCaching(options =>
        {
            // Memory缓存不需要序列化器，如果切换到Redis的话必须添加序列化器
            options.UseInMemory(configure =>
            {
                configure.MaxRdSecond = 5;
                configure.CacheNulls = false;
            }, "default");
            // options.UseRedis(it =>
            // {
                // it.DBConfig.Configuration= configuration["ConnectionStrings:RedisConnectionString"];
            // }, "redis");

            // options.WithJson("redis");
        });

        return serviceCollection;
    }
    /// <summary>
    /// 添加redis
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddRedisSetup(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        Check.NotNullOrEmpty(configuration["ConnectionStrings:RedisConnectionString"], "redis连接字符串不能为空");

        serviceCollection.TryAddSingleton(sp =>
        {
            ConnectionMultiplexer redis =
                ConnectionMultiplexer.Connect(configuration["ConnectionStrings:RedisConnectionString"]!);

            return redis;
        });

        return serviceCollection;
    }

    /// <summary>
    /// 使用Autofac注入服务
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder AddAutofacSetup(this IHostBuilder builder)
    {
        builder
            .UseServiceProviderFactory<
                ContainerBuilder>(new AutofacServiceProviderFactory())
            .ConfigureContainer(
                (Action<HostBuilderContext, ContainerBuilder>)((_, containerBuilder) =>
                    containerBuilder.RegisterModule(
                        new AutofacModuleRegister())));
        return builder;
    }

    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGenNewtonsoftSupport();

        serviceCollection.AddSwaggerGen(c =>
        {
            //添加Authorization
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Jwt请求 自带前缀Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT"
            });

            c.AddSecurityDefinition("Api", new OpenApiSecurityScheme
            {
                Description = "第三方应用请求Header 需要手动填写Api前缀",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "ApiKey",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "Api"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Api"
                        }
                    },
                    new List<string>()
                }
            });

            var _assemblies = new List<Assembly>()
            {
                Assembly.Load("LightApi.Api"), Assembly.Load("LightApi.Service"), Assembly.Load("LightApi.Domain")
            };
            foreach (var xmlAssembly in _assemblies)
            {
                var path = xmlAssembly.Location.Replace(".dll", ".xml");
                if (File.Exists(path))
                {
                    c.SchemaFilter<SwaggerEnumSchemaFilter>(path);
                    c.IncludeXmlComments(path, true);
                }
            }

            //允许上传文件
            c.OperationFilter<FileUploadFilter>();

            c.DocumentFilter<SwaggerEnumTypesDocumentFilter>();
        });

        return serviceCollection;
    }

    /// <summary>
    /// 请求记录
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddMiniProfilerSetup(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMiniProfiler(options => { options.RouteBasePath = "/profiler"; }).AddEntityFramework();

        return serviceCollection;
    }

    public static IMvcBuilder AddMvcSetup(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddRouting(options => options.LowercaseUrls = true);
        var mvcBuilder = serviceCollection.AddMvc(options =>
        {
             // autosave在绝大部分 filter 之前先执行，这样报错了有异常能被action上的aop捕获
            options.Filters.Add<AutoSaveAttribute>(100);
        });
        mvcBuilder.AddCustomJson();
        return mvcBuilder;
    }

    public static IMvcBuilder AddCustomJson(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddNewtonsoftJson(p =>
        {
            //数据格式首字母小写 不使用驼峰
            p.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //不使用驼峰样式的key
            // p.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //忽略循环引用
            p.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            p.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
            p.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            p.SerializerSettings.Formatting = Formatting.None;

            p.SerializerSettings.Converters = new List<JsonConverter>()
            {
                new DoubleTwoDigitalConverter(), new DecimalTwoDigitalConverter()
            };
        });

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy/MM/dd HH:mm:ss",
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            Converters =
                new List<JsonConverter>()
                {
                    new DoubleTwoDigitalConverter(), new DecimalTwoDigitalConverter()
                }
        };

        return mvcBuilder;
    }

    public static IServiceCollection AddEfCore(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // serviceCollection.AddInfrastructureEfCoreSqlServer<FbAppContext>(config =>
        // {
        //     config.UseSqlServer(configuration["ConnectionStrings:SqlServerConnectionString"],
        //         sqlOptions => { sqlOptions.MigrationsAssembly("LightApi.Api"); });
        //     config.EnableSensitiveDataLogging();
        //     config.EnableDetailedErrors();
        //     config.AddInterceptors(new SoftDeleteInterceptor());
        //     config.AddInterceptors(new SlowQueryLogInterceptor());
        //     config.ReplaceService<IMigrationsModelDiffer, MigrationsModelDifferWithoutForeignKey>();
        // }, Assembly.Load("LightApi.Domain"));
        
        


        return serviceCollection;
    }

    public static IServiceCollection AddEfCoreSqliteSetup(this IServiceCollection services)
    {
        var serviceType = typeof(IEntityInfo);
        var implType =
            typeof(FbAppContext).Assembly.ExportedTypes.FirstOrDefault(type => type.IsAssignableTo(serviceType));

        if (implType is null)
            throw new NotSupportedException(
                $"模型所在程序集必须继承 {nameof(IEntityInfo)} 接口,或者直接派生 {nameof(AbstractSharedEntityInfo)} 类");
        else
            services.AddSingleton(serviceType, implType);

        // services.TryAddScoped<IUnitOfWork, SqliteUnitOfWork<FbAppContext>>();

        services.AddScoped<AppDbContext>(sp => sp.GetService<FbAppContext>());

        services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));

        services.AddDbContext<FbAppContext>((sp, op) =>
        {
            var env = sp.GetService<IWebHostEnvironment>();
            // if (env.IsDevelopment())
            // {
            // op.UseSqlite("DataSource=file::memory:?cache=shared", b => b.MigrationsAssembly("FB.AppSrv"));
            // }
            // else
            // {
            op.UseSqlite( "Data Source=lightapi_data.db;", b => b.MigrationsAssembly("LightApi.Api"));

            // }
            // op.UseLoggerFactory(LoggerFactory.Create(builder =>
            // {
            // builder.AddFilter(_ => false);
            // }));
            // 自定义日志
            // op.LogTo(Log.Information,LogLevel.Information);
            op.EnableSensitiveDataLogging();
            op.EnableDetailedErrors();
            op.AddInterceptors(new SoftDeleteInterceptor());
            op.AddInterceptors(new SlowQueryLogInterceptor());
            op.ReplaceService<IMigrationsModelDiffer, MigrationsModelDifferWithoutForeignKey>();
        });
        return services;
    }
    public static WebApplicationBuilder AddSerilogSetup(this WebApplicationBuilder builder)
    {
        string logTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}]  {Message:lj}{NewLine}{Exception}";
        var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("ENV", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("System", "LightApi")
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Transaction", LogEventLevel.Debug)
            .MinimumLevel.Override("LightApi.Core.Authorization", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient.NacosClient", LogEventLevel.Warning)
            .MinimumLevel.Override("Nacos", LogEventLevel.Information)
            .WriteTo.Console(outputTemplate: logTemplate)
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"logs/lightapi_.log")
                , rollingInterval: RollingInterval.Day, outputTemplate: logTemplate);


        if (!string.IsNullOrWhiteSpace(builder.Configuration["Other:SeqUrl"]))
        {
            config.WriteTo.Seq(builder.Configuration["Other:SeqUrl"]);
        }

        if (!builder.Environment.IsDevelopment())
        {
            config.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
            config.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Transaction", LogEventLevel.Warning);
        }

        var logger = config.CreateLogger();
        Log.Logger = logger;
        builder.Host.UseSerilog(logger, dispose: true);
        return builder;
    }

    public static IServiceCollection AddNacos(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddNacosAspNet(configuration, section: "Nacos");
        return serviceCollection;
    }
}