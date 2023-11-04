using LightApi.Core;
using LightApi.Infra;
using LightApi.Infra.Helper;
using Microsoft.AspNetCore.Authorization;
using Prometheus;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddCustomSerilog();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    builder.Services.AddWindowsService()
        .AddInfra(builder)
#if DEBUG
#else
         .AddNacos(builder.Configuration)
#endif
        .AddCustomSwaggerGen()
        // .AddMonitor()
        .AddCustomCors()
        .AddCustomAuth()
        .AddWindowsService()
        // .AddEfCore(builder.Configuration)
        .AddEasyCaching(options =>
        {
            // Memory缓存不需要序列化器，如果切换到Redis的话必须添加序列化器
            options.UseInMemory(configure =>
            {
                configure.MaxRdSecond = 5;
                configure.CacheNulls = false;
            }, "default");
        })
        .AddFileLock()
        // .AddRedis(builder.Configuration)
        .AddHttpContextAccessor()
        .AddCustomMiniProfiler()
        .AddCustomMvc()
        .AddCustomJson()
        ;

    builder.Host.UseAutofac();
    
    builder.WebHost.ConfigureKestrel(it =>
    {
        // 上传文件大小500MB
        it.Limits.MaxRequestBodySize = 524288000;
    });

    var app = builder.Build();

    // DbInit.Init(app);

    App.Init(app);

    if (!app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(config =>
        {
            config.DocExpansion(DocExpansion.None);
            config.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            config.IndexStream = () =>
                new FileStream(Path.Combine(AppContext.BaseDirectory, "wwwroot", "swagger-index.html"), FileMode.Open,
                    FileAccess.Read);
        });
    }

    app.UseRouting();

    app.UseAuthentication();

    app.UseCors();

    app.UseAuthorization();

    app.UseHttpMetrics();

    if (!app.Environment.IsProduction())
        app.UseMiniProfiler();

    app.MapMetrics();

    app.MapControllers();

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