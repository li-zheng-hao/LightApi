using LightApi.Mongo.InternalExceptions;
using LightApi.Mongo.UnitOfWork;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace LightApi.Mongo;

public class OptimisticRetryExecutor
{
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

    public async Task Execute(Action<ExecuteOptions> options)
    {
        var executeOptions = new ExecuteOptions();
        options(executeOptions);
        try
        {
            await executeOptions.Pipeline.ExecuteAsync(async _ =>
            {
                await executeOptions.Func();
            });
        }
        catch (Exception e)
        {
            executeOptions.Logger.LogError(e, $"工作单元异常:{e.Message}");
            throw;
        }

    }

    public class ExecuteOptions
    {
        /// <summary>
        /// 功能区名称
        /// </summary>
        /// <value></value>
        public string SectionName { get; set; }
        public Func<Task> Func { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public ResiliencePipeline Pipeline { get; set; }
        public ILogger Logger { get; set; }
        public bool UseCapPublisher { get; set; }
    }
}