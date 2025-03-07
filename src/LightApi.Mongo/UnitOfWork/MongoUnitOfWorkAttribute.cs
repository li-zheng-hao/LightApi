using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightApi.Mongo.UnitOfWork;

public class MongoUnitOfWorkAttribute : ActionFilterAttribute
{

    /// <summary>
    ///  开启MongoDB的工作单元事务
    /// </summary>
    /// <param name="useCapPublisher">是否使用CAP</param>
    public MongoUnitOfWorkAttribute(bool useCapPublisher = false)
    {
        UseCapPublisher = useCapPublisher;
    }

    /// <summary>
    /// 是否使用CAP
    /// </summary>
    public bool UseCapPublisher { get; set; }

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var uow = context.HttpContext.RequestServices.GetRequiredService<IMongoUnitOfWork>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<
                ILogger<MongoUnitOfWorkAttribute>
            >();
        try
        {

            uow.StartTransaction(context.HttpContext.RequestServices, UseCapPublisher);
            var executed = await next();
            if (executed.Exception != null)
                await uow.RollbackAsync();
            else
                await uow.CommitAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MongoDB工作单元执行失败，回滚事务");
            await uow.RollbackAsync();
            throw;
        }
    }
}
