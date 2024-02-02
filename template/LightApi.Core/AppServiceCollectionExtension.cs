using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Asp.Versioning;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using LightApi.Core.Aop;
using LightApi.Core.Authorization;
using LightApi.Core.Authorization.Api;
using LightApi.Core.Authorization.Hybrid;
using LightApi.Core.Authorization.Jwt;
using LightApi.Core.Autofac;
using LightApi.Core.Converter;
using LightApi.Core.FileProvider;
using LightApi.Core.Options;
using LightApi.Core.Swagger;
using LightApi.Domain;
using LightApi.EFCore;
using LightApi.EFCore.Config;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Interceptors;
using LightApi.EFCore.Repository;
using LightApi.Infra;
using LightApi.Infra.DependencyInjections;
using LightApi.Infra.Extension;
using Masuit.Tools.Systems;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Nacos.AspNetCore.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus.SystemMetrics;
using Prometheus.SystemMetrics.Collectors;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

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

    public static IServiceCollection AddInfraSetup(this IServiceCollection serviceCollection, WebApplicationBuilder builder)
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
        serviceCollection.AddAuthentication(option =>
            {
                // 默认混合方式 支持jwt cookie api token三种方式，优先级为cookie>>jwt>>api
                option.DefaultScheme = CustomAuthorizationSchemes.HybridSchemeName;
            })
            .AddScheme<CustomHybridAuthSchemeOptions, CustomHybridAuthHandler>(
                CustomAuthorizationSchemes.HybridSchemeName,
                i => { })
            .AddScheme<CustomJwtAuthSchemeOptions, CustomJwtAuthHandler>(CustomAuthorizationSchemes.JwtSchemeName,
                i => { })
            .AddScheme<CustomApiAuthSchemeOptions, CustomApiAuthHandler>(CustomAuthorizationSchemes.ApiSchemeName,
                i => { })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                // 滑动过期 24小时
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
                // 系统名称
                options.Cookie.Name = "LightApi";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
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
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IHostBuilder AddAutofacSetup(this IHostBuilder builder,IConfiguration configuration)
    {
        builder
            .UseServiceProviderFactory<
                ContainerBuilder>(new AutofacServiceProviderFactory())
            .ConfigureContainer(
                (Action<HostBuilderContext, ContainerBuilder>)((_, containerBuilder) =>
                    containerBuilder.RegisterModule(
                        new AutofacModuleRegister(configuration))));
        return builder;
    }
    /// <summary>
    /// 版本控制
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiVersionSetup(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
                opt.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddApiExplorer(options =>
            {
                // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
        return serviceCollection;
    }
    public static IServiceCollection AddSwaggerGenSetup(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGenNewtonsoftSupport();
        serviceCollection.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

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
            // Add a custom operation filter which sets default values
            c.OperationFilter<SwaggerDefaultValues>();
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
        services.AddInfrastructureEfCoreSqlite<FbAppContext>((sp, op) =>
        {
            var dbPath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lightapi_data.db");
            op.UseSqlite( $"Data Source={dbPath};", b => b.MigrationsAssembly("LightApi.Api"));
            op.EnableSensitiveDataLogging();
            op.EnableDetailedErrors();
            op.AddInterceptors(new SlowQueryLogInterceptor());
            op.ReplaceService<IMigrationsModelDiffer, MigrationsModelDifferWithoutForeignKey>();
        },typeof(EntityInfo));
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