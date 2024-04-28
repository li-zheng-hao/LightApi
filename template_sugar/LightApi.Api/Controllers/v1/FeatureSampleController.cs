using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using LightApi.Infra.Extension;
using LightApi.Infra.Helper;
using LightApi.Infra.InfraException;
using LightApi.Infra.ModelValidator;
using LightApi.Infra.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace LightApi.Api.Controllers;

/// <summary>
/// 示例
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class FeatureSampleController : ControllerBase
{
    
    [HttpPost("test-json")]
    public IActionResult TestJson([FromBody] TestJsonRequest request)
    {
        return Ok(request);
    }

    public class TestJsonRequest
    {
        [MinLength(1000)]
        [RequiredEx]
        public string Name { get; set; }
        public int Age { get; set; }
    }

    /// <summary>
    /// 下载excel
    /// </summary>
    /// <returns></returns>
    [HttpGet("excel-sample")]
    public async Task<IActionResult> Test()
    {

        var table = ExcelHelper.CreateDatatable("第一", "第二");
        table.AddRow("a",1);
        table.AddRow("b",2);
        table.AddRow("c",3);
        var ms=new MemoryStream();
        await ms.SaveAsAsync(table);
        ms.Seek(0, SeekOrigin.Begin);
        return File(ms,"application/octet-stream","test.xlsx");
    }
    
    [HttpPost("validate-excel")]
    public IActionResult ValidateExcel(IFormFile file)
    {
        var table = ExcelHelper.GetValues(file.OpenReadStream());
        var passed=ExcelHelper.ValidateHeaders(table,"第一", "第二");
        Check.ThrowIf(!passed, "表头不正确");
        ExcelHelper.ThrowIfNotInt(table, 1);
        return Ok();
    }
   
    
}

