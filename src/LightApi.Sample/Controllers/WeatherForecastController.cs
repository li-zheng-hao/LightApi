using System.Diagnostics;
using LightApi.Infra.AOP.Attributes;
using LightApi.Mongo.Entities;
using LightApi.Mongo.Extensions;
using LightApi.Mongo.InternalExceptions;
using LightApi.Mongo.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace LightApi.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }
    [LogAction("调用")]
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        InternalCall();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    private void InternalCall()
    {
        var newActivity=Activity.Current?.Source.StartActivity("InternalCall", ActivityKind.Internal);
        _logger.LogInformation("内部调用");
        newActivity?.Stop();
    }

    [HttpPost("test1")]
    [MongoUnitOfWork(enableOptimisticLock:true)]
    public async Task<IActionResult> Test1([FromServices]DBContext dbContext)
    {
        var item=dbContext.Queryable<ModelTest>()
            .FirstOrDefault(it => it.Id == "66dff1154e54b7709104d50e");
        item.Name = "test1"+DateTime.Now.ToString();
        await dbContext.SaveWithOptimisticAsync(item);
        return Ok();
    }

    private static int i = 0;
    [HttpPost("test2")]
    [MongoUnitOfWork(enableOptimisticLock:true)]
    public async Task<IActionResult> Test2([FromServices]DBContext dbContext)
    {
        // var item=dbContext.Queryable<ModelTest>()
            // .FirstOrDefault(it => it.Id == "66dff1154e54b7709104d50e");
        // item.Name = "test2"+DateTime.Now.ToString();
        // await Task.Delay(5000);
        // await dbContext.SaveWithOptimisticAsync(item);
        if(i++%2==0)
            throw new MongoOptimisticException();
        return Ok();
    }
    
    internal class ModelTest:MongoEntity,IOptimisticLock
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}