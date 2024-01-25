using Asp.Versioning;
using EasyNetQ.Consumer;
using LightApi.Core.FileProvider;
using LightApi.Core.Helper;
using LightApi.Infra;
using LightApi.Infra.AOP.Attributes;
using LightApi.Infra.Helper;
using LightApi.Infra.InfraException;
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
public class FileController : ControllerBase
{
    [HttpGet("test")]
    public string test()
    {
        throw new Exception("custom exception");
        return "from v1";
    }
    /// <summary>
    /// 上传
    /// </summary>
    /// <param name="fileDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm]UploadDto fileDto)
    {
        var fileProvider = App.GetNamedService<IFileProvider>("Local");
        var result=await fileProvider!.SaveFile(fileDto.File);
        return Ok(result);
    }

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Download([FromQuery]string url)
    {
        var fileProvider = App.GetNamedService<IFileProvider>("Local");
        var s=await fileProvider!.GetStream(url);
        return File(s,"application/octet-stream",Path.GetFileName(url));
    }
    
}

public class UploadDto
{
    public IFormFile File { get; set; }
}