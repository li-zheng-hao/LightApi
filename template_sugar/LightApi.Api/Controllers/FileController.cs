using LightApi.Core.FileProvider;
using LightApi.Infra;
using Microsoft.AspNetCore.Mvc;

namespace LightApi.Api.Controllers;

/// <summary>
/// 文件
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class FileController : ControllerBase
{
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