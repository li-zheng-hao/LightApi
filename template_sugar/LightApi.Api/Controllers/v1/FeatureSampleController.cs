using Asp.Versioning;
using EasyNetQ.Consumer;
using LightApi.Core.FileProvider;
using LightApi.Core.Helper;
using LightApi.Infra;
using LightApi.Infra.AOP.Attributes;
using LightApi.Infra.Helper;
using LightApi.Infra.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;

namespace LightApi.Api.Controllers;

/// <summary>
/// 文件
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class FeatureSampleController : ControllerBase
{
    [HttpPost("mq-test")]
    public async Task<IActionResult> MqTest([FromServices]RabbitMqManager manager)
    {
        await manager.SubscribeAsync<string>(configure =>
        {
            configure.Handler = async (msg, info) =>
            {
                Console.WriteLine(msg);
                await Task.Delay(1000);
                return AckStrategies.Ack;
            };
            configure.Topic = "test";
        });
       
        return Ok();
    }
    [HttpPost("process-invoke")]
    public async Task<IActionResult> ProcessInvoke([FromServices]RabbitMqManager manager)
    {
        var res=await ProcessInvokeHelper.ExecuteAsync<ParamDto, ParamDto>(config =>
        {
            config.ExecutePath =
                "E:\\Code\\LightApi\\template_sugar\\LightApi.ConsoleSample\\bin\\Debug\\net7.0\\LightApi.ConsoleSample.exe";
            // config.JsonParam = dto;
            config.Timeout = 1;
        });
        return Ok(res);
    }
   
    
}

