using LightApi.Domain;
using LightApi.Infra.AOP.Attributes;
using LightApi.Infra.InfraException;
using LightApi.Service;
using LightApi.SqlSugar;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Check = LightApi.Infra.Extension.Check;

namespace LightApi.Api.Controllers;

/// <summary>
/// 注释
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class SugarController : ControllerBase
{

    private readonly ILogger<SugarController> _logger;
    private readonly IBaseRepository<School> _repository;
    private readonly StudentService _studentService;

    public SugarController(ILogger<SugarController> logger,IBaseRepository<School> repository,StudentService studentService)
    {
        _logger = logger;
        _repository = repository;
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
        _repository.Insert(school);
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
        var data=await _studentService.Repository.ChangeRepository<BaseRepository<Student>>()
            .AsQueryable().Where(it => ids.Contains(it.Id)).ToListAsync();
        return Ok(data);
    }
    /// <summary>
    /// 新增学生
    /// </summary>
    /// <returns></returns>
    [HttpPost()]
    [LogAction("AddStudent")]
    public async Task<IActionResult> AddStudent([FromBody] Student student)
    {
        await _repository.ChangeRepository<BaseRepository<Student>>()!.InsertAsync(student);
        return Ok();
    }
    /// <summary>
    /// 更新学生
    /// </summary>
    /// <returns></returns>
    [HttpPost()]
    [LogAction("UpdateStudent")]
    public async Task<IActionResult> UpdateStudent([FromBody] Student student)
    {
        var data=await _repository.ChangeRepository<BaseRepository<Student>>()!.UpdateAsync(student);
        
        return Ok(data);
    }
    /// <summary>
    /// 删除学生
    /// </summary>
    /// <returns></returns>
    [HttpPost()]
    [LogAction("DeleteStudent")]
    public async Task<IActionResult> DeleteStudent([FromQuery]long id)
    {
        var data=await _repository.ChangeRepository<BaseRepository<Student>>()!.DeleteByIdAsync(id);

        return Ok(data);
    }

    /// <summary>
    /// 测试工作单元
    /// </summary>
    /// <param name="school"></param>
    /// <returns></returns>
    /// <exception cref="BusinessException"></exception>
    [SugarUnitOfWork]
    [HttpPost("")]
    public async Task<IActionResult> TestRollback([FromBody]School school)
    {
        var result=await _studentService.Repository.ChangeRepository<BaseRepository<School>>().InsertAsync(school);
        throw new BusinessException("11");
        return Ok();
    }
    
    
    /// <summary>
    /// 导航查询
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<Student>> Include()
    {
        var res=await _studentService.Repository.AsQueryable().Includes(it => it.Schools)
            .ToListAsync();
        return res;
    }

}