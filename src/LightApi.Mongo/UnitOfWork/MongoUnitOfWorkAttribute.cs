using System.Runtime.InteropServices.ComTypes;
using DotNetCore.CAP;
using LightApi.Mongo.InternalExceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace LightApi.Mongo.UnitOfWork;

public class MongoUnitOfWorkAttribute : ActionFilterAttribute
{
    private ILogger<MongoUnitOfWorkAttribute> _logger;

    private ICapPublisher? _publisher;

    private IMongoUnitOfWork _uow;

    public bool EnableOptimisticLock { get; set; }
    /// <summary>
    /// 乐观锁最大重试次数
    /// </summary>
    public int OptimisticLockRetryCount { get; set; } = 3;
    /// <summary>
    ///  开启MongoDB的工作单元事务
    /// </summary>
    /// <param name="useCapPublisher">是否使用CAP</param>
    public MongoUnitOfWorkAttribute(bool useCapPublisher = false, bool enableOptimisticLock = false)
    {
        UseCapPublisher = useCapPublisher;
        EnableOptimisticLock = enableOptimisticLock;
        RetryPipeLine = GenerateOptimisticRetryPipeline(OptimisticLockRetryCount);
    }

    /// <summary>
    /// 是否使用CAP
    /// </summary>
    public bool UseCapPublisher { get; set; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<MongoUnitOfWorkAttribute>>();
        _uow = context.HttpContext.RequestServices.GetRequiredService<IMongoUnitOfWork>();
        if (!EnableOptimisticLock)
        {
            var actionExecutedContext = await next();
            if (actionExecutedContext.Exception is not null)
            {
                if (!(actionExecutedContext.Exception is MongoOptimisticException))
                    _logger.LogError(actionExecutedContext.Exception, $"工作单元回滚,异常信息：{actionExecutedContext.Exception.Message}");
                await _uow.RollbackAsync();
            }
            else
            {
                await _uow.CommitAsync();
            }
        }
        else
        {
            var resilienceContext = ResilienceContextPool.Shared.Get();

            try
            {
                await RetryPipeLine.ExecuteAsync(async _ =>
                {
                    resilienceContext.Properties.Set(new ResiliencePropertyKey<string>("path"),
                        context.HttpContext.Request.Path.ToString());
                    _uow.StartTransaction(context.HttpContext.RequestServices, UseCapPublisher);

                    var actionExecutedContext = await next();

                    if (actionExecutedContext.Exception is not null)
                    {
                        if (!(actionExecutedContext.Exception is MongoOptimisticException))
                        {
                            _logger.LogError(actionExecutedContext.Exception, $"工作单元回滚,异常信息：{actionExecutedContext.Exception.Message}");
                            actionExecutedContext.Exception = null;
                        }
                        await _uow.RollbackAsync();
                    }
                    else
                        await _uow.CommitAsync();
                }, resilienceContext);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"工作单元异常:{e.Message}");
                throw;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(resilienceContext);
            }
        }
    }



    private ResiliencePipeline GenerateOptimisticRetryPipeline(int optimisticLockRetryCount)
    {
        // For advanced control over the retry behavior, including the number of attempts,
        // delay between retries, and the types of exceptions to handle.
        var optionsComplex = new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<MongoOptimisticException>(),
            MaxRetryAttempts = optimisticLockRetryCount,
            DelayGenerator = (arg) =>
            {
                var randomMillSeconds = Random.Shared.Next(50, 1000);
                return new ValueTask<TimeSpan?>(TimeSpan.FromMilliseconds(randomMillSeconds));
            },
            OnRetry = result =>
            {
                result.Context.Properties.TryGetValue(new ResiliencePropertyKey<string>("path"), out var path);
                _logger.LogWarning($"乐观锁冲突,重试次数{result.AttemptNumber},重试延迟{result.RetryDelay},路径:{path}");
                return ValueTask.CompletedTask;
            },
        };
        var pipeline = new ResiliencePipelineBuilder().AddRetry(optionsComplex).Build();
        return pipeline;
    }


}
