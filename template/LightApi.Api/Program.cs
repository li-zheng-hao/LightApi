using LightApi.Core;
using LightApi.Core.Authorization;
using LightApi.Core.Authorization.Hybrid;
using LightApi.Core.Options;
using LightApi.Domain;
using LightApi.Infra;
using LightApi.Infra.DependencyInjections;
using LightApi.Infra.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region 注入配置

    builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));
    builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection(FileStorageOptions.SectionName));
    builder.Services.Configure<SecretOptions>(builder.Configuration.GetSection(SecretOptions.SectionName));
    builder.Services.Configure<ThirdPartOptions>(builder.Configuration.GetSection(ThirdPartOptions.SectionName));

    #endregion
    
    #region 注册服务

    builder.Services.AddControllers();
    builder.Services.AddSwaggerGenSetup();
    builder.Services.AddEndpointsApiExplorer();

    // 支持接口Url版本控制
    builder.Services.AddApiVersionSetup();
    // Serilog日志
    builder.AddSerilogSetup(); 
    
    // Windows服务
    builder.Services.AddWindowsService(); 
    
    // 基础框架
    builder.Services.AddInfraSetup(builder); 
    
#if DEBUG
#else
    // 服务注册中心
    builder.Services.AddNacos(builder.Configuration) 
#endif
    
    //Prometheus监控
    // builder.Services.AddMonitorSetup(); 
    
    // 跨域
    builder.Services.AddCorsSetup(); 
    
    // 文件服务
    builder.Services.AddFileProviderSetup(); 
    
    // 权限认证
    builder.Services.AddAuthorizeSetup(); 
    
    // EFCore
    builder.Services.AddEfCoreSqliteSetup(); 
    
    // 缓存
    builder.Services.AddEasyCachingSetup(); 
    
    // 分布式锁
    builder.Services.AddDistributedLockSetup(); 
    
    // Redis
    // builder.Services.AddRedisSetup(builder.Configuration); 
    
    builder.Services.AddHttpContextAccessor();
    
    // Miniprofiler
    builder.Services.AddMiniProfilerSetup(); 
    
    // MVC配置
    builder.Services.AddMvcSetup(); 

    // IOC容器 及批量注入
    builder.Host.AddAutofacSetup(builder.Configuration); 

    // Kestrel配置
    builder.WebHost.ConfigureKestrel(it =>
    {
        // 上传文件大小500MB
        it.Limits.MaxRequestBodySize = 524288000;
    }); 

    #endregion

    var app = builder.Build();

    #region 中间件

    // 数据库迁移脚本执行
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<FbAppContext>();
        // 如果多实例 这里需要加redis锁 或者把迁移拆分出来
        db.Database.Migrate();
    }

    // Swagger
    if (!app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(config =>
        {
            var descriptions = app.DescribeApiVersions();
            // Build a swagger endpoint for each discovered API version
            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                config.SwaggerEndpoint(url, name);
            }
            config.DocExpansion(DocExpansion.None);
            config.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            config.IndexStream = () =>
                new FileStream(Path.Combine(AppContext.BaseDirectory, "wwwroot", "swagger-index.html"), FileMode.Open,
                    FileAccess.Read);
           
        });
    }

    // 基础框架
    app.UseInfrastructure();

    app.UseRouting();

    app.UseAuthentication();

    app.UseCors();

    app.UseAuthorization();

    app.UseHttpMetrics();

    if (!app.Environment.IsProduction())
        app.UseMiniProfiler();

    app.MapMetrics();

    // 全局开启权限校验
    app.MapControllers().RequireAuthorization();

    #endregion

    app.Run();
}
catch (HostAbortedException)
{
    // ignore
}
catch (Exception exception)
{
    Log.Logger.Fatal(exception, $"程序启动失败 {exception.Message}");
    Log.CloseAndFlush();
    Environment.Exit(1);
}