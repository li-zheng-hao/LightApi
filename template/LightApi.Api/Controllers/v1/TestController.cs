using System.ComponentModel.DataAnnotations;
using LightApi.Domain;
using LightApi.Domain.Entities;
using LightApi.Infra.AOP.Attributes;
using LightApi.Infra.Extension;
using LightApi.Service;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LightApi.Api.Controllers;

/// <summary>
/// 注释
/// </summary>
[ApiController]

[Route("api/[controller]/[action]")]
public class TestController : ControllerBase
{

    private readonly ILogger<TestController> _logger;
    private readonly FbAppContext _fbAppContext;
    private readonly StudentService _studentService;

    public TestController(ILogger<TestController> logger,FbAppContext fbAppContext,StudentService studentService)
    {
        _logger = logger;
        _fbAppContext = fbAppContext;
        _studentService = studentService;
    }
    
    /// <summary>
    /// 获取学校
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [LogAction("获取学校")]
    public IActionResult GetSchool()
    {
        throw new Exception("11");
        return Ok(_fbAppContext.Entities<School>().ToList());
    }
    /// <summary>
    /// 新增学校
    /// </summary>
    /// <returns></returns>
    [HttpPost()]
    [LogAction("新增学校")]
    public IActionResult AddSchool([FromBody] School school)
    {
        _fbAppContext.Add(school);
        _fbAppContext.SaveChanges();
        return Ok();
    }
    /// <summary>
    /// 获取学生
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [LogAction("获取学生")]
    public async Task<IActionResult> GetStudent([FromQuery]List<long> ids)
    {
        return Ok(_fbAppContext.Entities<Student>().AsQueryable().Include(it=>it.School).ToList());
    }
    /// <summary>
    /// 新增学生
    /// </summary>
    /// <returns></returns>
    [HttpPost()]
    [LogAction("AddStudent")]
    public IActionResult AddStudent([FromBody] Student student)
    {
        _fbAppContext.Add(student);
        _fbAppContext.SaveChanges();
        return Ok();
    }
    /// <summary>
    /// 更新学生
    /// </summary>
    /// <returns></returns>
    [HttpPost()]
    [LogAction("UpdateStudent")]
    public IActionResult UpdateStudent([FromBody] Student student)
    {
        var entity = _fbAppContext.AsQueryable<Student>().AsTracking().First(it => it.Id == student.Id);
        student.Adapt(entity);
        _fbAppContext.SaveChanges();
        return Ok();
    }
    /// <summary>
    /// 删除学生
    /// </summary>
    /// <returns></returns>
    [HttpPost()]
    [LogAction("DeleteStudent")]
    public async Task<IActionResult> DeleteStudent([FromQuery]long id)
    {
        var entity = _fbAppContext.AsQueryable<Student>().Where(it=>it.Id==id).First();
        _fbAppContext.Remove(entity);
        _fbAppContext.SaveChanges();
        return Ok();
    }
}