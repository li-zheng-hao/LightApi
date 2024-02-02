using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LightApi.Core.Authorization;
using LightApi.Core.Authorization.Hybrid;
using LightApi.Domain;
using LightApi.Domain.Entities;
using LightApi.EFCore.Repository;
using LightApi.Infra.AOP.Attributes;
using LightApi.Infra.Extension;
using LightApi.Service;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IEfRepository<School> _repository;

    public TestController(ILogger<TestController> logger,FbAppContext fbAppContext,StudentService studentService,
        IEfRepository<School> repository)
    {
        _logger = logger;
        _fbAppContext = fbAppContext;
        _studentService = studentService;
        _repository = repository;
    }

    [HttpPost]
    public async Task LoginCookie()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "demo"),
            new Claim(ClaimTypes.Role, "admin,123"),
        };
            
        var claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        
    }
    /// <summary>
    /// 获取学校
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [LogAction("获取学校")]
    [Authorize]
    public IActionResult GetSchool()
    {
        return Ok(_repository.AsQueryable().ToList());
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