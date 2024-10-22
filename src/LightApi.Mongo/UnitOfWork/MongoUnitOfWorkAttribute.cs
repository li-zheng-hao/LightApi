using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightApi.Mongo.UnitOfWork;

public class MongoUnitOfWorkAttribute : ActionFilterAttribute
{
    private ILogger<MongoUnitOfWorkAttribute> _logger;

    private ICapPublisher? _publisher;
    
    private IMongoUnitOfWork _uow;

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

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<MongoUnitOfWorkAttribute>>();
            _uow=context.HttpContext.RequestServices.GetRequiredService<IMongoUnitOfWork>();
            _uow.StartTransaction(context.HttpContext.RequestServices,UseCapPublisher);
            var executed=await next();
            if (executed.Exception!=null)
                await _uow.RollbackAsync();
            else
                await _uow.CommitAsync();
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
   
}
