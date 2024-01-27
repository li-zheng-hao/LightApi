using LightApi.EFCore;
using LightApi.EFCore.Repository;
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
        var repository = HttpContext.RequestServices.GetService<IEfRepository<SampleModel>>();


        var rr=repository.GetDbSet().ToList();

        var repository2 = HttpContext.RequestServices.GetService<IEfRepository<SampleModel2, DemoDbContext2>>();

        return new
        {
            Repo1Result = repository!.AsQueryable<SampleModel>().ToList(),
            Repo2Result = repository2!.AsQueryable<SampleModel2>().ToList()
        };
    }

    [HttpPost("[action]"), UnitOfWork(DbContextType = typeof(DemoDbContext))]
    public IActionResult Trans1()
    {
        var repository = HttpContext.RequestServices.GetService<IEfRepository<SampleModel>>();
        repository!.Add(new SampleModel()
        {
            Name = Guid.NewGuid().ToString()
        });
        repository.SaveChanges();
        return Ok();
    }

    [HttpPost("[action]"), UnitOfWork(DbContextType = typeof(DemoDbContext2))]
    public IActionResult Trans2()
    {
        var repository2 = HttpContext.RequestServices.GetService<IEfRepository<SampleModel2, DemoDbContext2>>();

        repository2!.Add(new SampleModel2()
        {
            Name = Guid.NewGuid().ToString()
        });
        repository2.SaveChanges();
        throw new Exception("a");
        return Ok();
    }
    [HttpPost("[action]"), UnitOfWork(DbContextType = typeof(DemoDbContext)), UnitOfWork(DbContextType = typeof(DemoDbContext2))]
    public IActionResult Trans3([FromServices]SqlServerUnitOfWork<DemoDbContext2> uow2)
    {
        var repository = HttpContext.RequestServices.GetService<IEfRepository<SampleModel>>();
        repository!.Add(new SampleModel()
        {
            Name = Guid.NewGuid().ToString()
        });
        repository.SaveChanges();
        
        var repository2 = HttpContext.RequestServices.GetService<IEfRepository<SampleModel2, DemoDbContext2>>();

        repository2!.Add(new SampleModel2()
        {
            Name = Guid.NewGuid().ToString()
        });
        repository2.SaveChanges();
        uow2.Rollback();
        return Ok();
    }
}