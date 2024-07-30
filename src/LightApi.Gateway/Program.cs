using System.Security.Claims;
using LightApi.Gateway.GrayLoadBalancing;
using LightApi.Gateway.LoadBalance;
using LightApi.Gateway.Swagger;
using LightApi.Gateway.YarpNacosProxy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Nacos.V2.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Swagger;
using Yarp.ReverseProxy.Swagger.Extensions;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// YarpNacosStore 
// 路由配置
builder.Configuration.AddJsonFile("route.json", false, true);

builder.Services.AddCookieAuthSetup("test_demo_key");
builder.Services.AddAuthorization(it=>
{
    it.AddPolicy("auth", configure => { configure.RequireAuthenticatedUser(); });
    it.DefaultPolicy = it.GetPolicy("auth")!;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSingleton<NacosServiceStore>();
builder.Services.AddSwaggerGen();

builder.Services.AddNacosV2Config(opt =>
{
    opt.EndPoint = string.Empty;
    opt.ServerAddresses = new List<string>() { "" };
    opt.Namespace = "";
    opt.UserName = "";
    opt.Password = "";
    // swich to use http or rpc
    opt.ConfigUseRpc = false;
});
// 添加Nacos相关服务
builder.Services.AddNacosV2Naming(opt =>
{
    opt.EndPoint = string.Empty;
    opt.ServerAddresses = new List<string>() { "" };
    opt.Namespace = "";
    opt.UserName = "";
    opt.Password = "";

    // swich to use http or rpc
    opt.NamingUseRpc = false;
});

var routeConfiguration = builder.Configuration.GetSection("ReverseProxy");

builder.Services.AddReverseProxy()
    .LoadFromConfig(routeConfiguration)
    .AddNacosDestinationResolver()
    .AddSwagger(routeConfiguration)
    .AddTransforms(builderContext =>
    {
        // Conditionally add a transform for routes that require auth.
        if (!string.IsNullOrEmpty(builderContext.Route.AuthorizationPolicy))
        {
            builderContext.AddRequestTransform(async transformContext =>
            {
                var userId=transformContext.HttpContext.User.FindFirst("uid");
                transformContext.ProxyRequest.Headers.Add("user_id", userId?.Value);
            });
        }
    });
    ;
builder.Services.AddSingleton<ILoadBalancingPolicy, GrayWeightLoadBalancingPolicy>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
        options.ConfigureSwaggerEndpoints(config);
    });
}
app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.UseSessionAffinity();
    proxyPipeline.UseLoadBalancing();
    proxyPipeline.UsePassiveHealthChecks();
});
app.Run();