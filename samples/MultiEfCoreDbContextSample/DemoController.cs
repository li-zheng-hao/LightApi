using LightApi.EFCore;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.SqlServer.Transaction;
using Microsoft.AspNetCore.Mvc;
using MultiEfCoreDbContextSample.Database;

namespace MultiEfCoreDbContextSample;

[ApiController]
[Route("demo")]
public class DemoController : ControllerBase
{
    [HttpGet("1"),]
    public dynamic Demo1()
    {
        var demoDbContext = HttpContext.RequestServices.GetService<DemoDbContext>();


        var rr=demoDbContext.AsQueryable<SampleModel>().ToList();
        var demoDbContext2 = HttpContext.RequestServices.GetService<DemoDbContext2>();
        var rr2=demoDbContext2.AsQueryable<SampleModel2>().ToList();


        return new
        {
            Repo1Result = rr,
            Repo2Result = rr2
        };
    }

    [HttpPost("[action]"), UnitOfWork(DbContextType = typeof(DemoDbContext))]
    public IActionResult Trans1()
    {
        var dbContext = HttpContext.RequestServices.GetService<DemoDbContext>();
        dbContext!.Add(new SampleModel()
        {
            Name = Guid.NewGuid().ToString()
        });
        dbContext.SaveChanges();
        return Ok();
    }

    [HttpPost("[action]"), UnitOfWork(DbContextType = typeof(DemoDbContext2))]
    public IActionResult Trans2()
    {
        var demoDbContext2= HttpContext.RequestServices.GetService<DemoDbContext2>();

        demoDbContext2!.Add(new SampleModel2()
        {
            Name = Guid.NewGuid().ToString()
        });
        demoDbContext2.SaveChanges();
        throw new Exception("a");
        return Ok();
    }
    [HttpPost("[action]"), UnitOfWork(DbContextType = typeof(DemoDbContext)), UnitOfWork(DbContextType = typeof(DemoDbContext2))]
    public IActionResult Trans3([FromServices]SqlServerUnitOfWork<DemoDbContext2> uow2)
    {
        var demoDbContext = HttpContext.RequestServices.GetService<DemoDbContext>();
        demoDbContext!.Add(new SampleModel()
        {
            Name = Guid.NewGuid().ToString()
        });
        demoDbContext.SaveChanges();
        
        var demoDbContext2 = HttpContext.RequestServices.GetService<DemoDbContext2>();

        demoDbContext2!.Add(new SampleModel2()
        {
            Name = Guid.NewGuid().ToString()
        });
        demoDbContext2.SaveChanges();
        uow2.Rollback();
        return Ok();
    }
}