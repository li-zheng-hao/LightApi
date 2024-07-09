using System.Diagnostics;
using LightApi.Infra.AOP.Attributes;
using Microsoft.AspNetCore.Mvc;

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
}