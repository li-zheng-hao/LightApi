using LightApi.Infra.Rpc;
using Microsoft.AspNetCore.Mvc;
using Nacos.AspNetCore.V2;
using NacosRpcSample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddNacosAspNet(builder.Configuration);
builder.Services.AddRefitRpcClient<App1ApiClient>(c =>
{
    c.Host = "app1";
    c.ServiceDiscoveryType = ServiceDiscoveryType.Nacos;
    c.UseTls = false;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/ping", () => "Hello World! from app2 api/ping");
app.MapGet("/", () => "Hello World! from  app2 /");
app.MapGet("/test", ([FromServices] App1ApiClient apiClient) => apiClient.GetHelloWorld());
app.MapGet("/test2", ([FromServices] App1ApiClient apiClient) => apiClient.GetHelloWorld2());
app.Run();
