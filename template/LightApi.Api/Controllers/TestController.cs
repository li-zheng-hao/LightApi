using LightApi.Infra.AOP.Attributes;
using LightApi.Infra.Extension;
using LightApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace LightApi.Api.Controllers;

/// <summary>
/// 注释
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : ControllerBase
{

    private readonly ILogger<TestController> _logger;
    private readonly TestService _testService;

    public TestController(ILogger<TestController> logger,TestService testService)
    {
        _logger = logger;
        _testService = testService;
    }
    /// <summary>
    /// 接口
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [LogAction]
    public IActionResult Get()
    {
        return Ok(_testService.Test());
    }
    /// <summary>
    /// 接口
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [OpLog("Get2")]
    public IActionResult Get2()
    {
        Check.ThrowIf(true,"eerrr");
        return Ok();
    }
}