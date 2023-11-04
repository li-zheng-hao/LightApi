#if NET6_0_OR_GREATER

using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightApi.Mongo.UnitOfWork;

public class MongoUnitOfWorkAttribute : ResultFilterAttribute
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

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        try
        {
            _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<MongoUnitOfWorkAttribute>>();
            _uow=context.HttpContext.RequestServices.GetRequiredService<IMongoUnitOfWork>();
            _uow.StartTransaction(context.HttpContext.RequestServices,UseCapPublisher);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"工作单元异常 { e.Message}");
            throw;
        }
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
        try
        {
            if (context.Exception is null)
            {
                _uow.CommitAsync().GetAwaiter().GetResult();
            }
            else
            {
                _logger.LogError(context.Exception, $"工作单元回滚,异常信息：{context.Exception.Message}");
                _uow.RollbackAsync().GetAwaiter().GetResult();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"工作单元异常 { e.Message}");
    
            throw;
        }
    }
   
}
#endif
