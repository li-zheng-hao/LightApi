using Polly;
using Polly.Timeout;

namespace LightApi.Core.Rpc;

public class PollyPolicyManager
{
    /// <summary>
    /// 默认策略 重试+超时+熔断
    /// </summary>
    /// <returns></returns>
    public static List<IAsyncPolicy<HttpResponseMessage>> GenerateDefaultPolicies()
    {
        //隔离策略
        //var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(10, 100);

        //回退策略
        //回退也称服务降级，用来指定发生故障时的备用方案。
        //目前用不上
        //var fallbackPolicy = Policy<string>.Handle<HttpRequestException>().FallbackAsync("substitute data");

        //重试策略,超时或者API返回502 503的错误,重试4次。
        //重试次数会统计到失败次数
        var retryPolicy = Policy.Handle<TimeoutRejectedException>()
            .OrResult<HttpResponseMessage>(response => (int)response.StatusCode == 502 || (int)response.StatusCode ==503)
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
            });
        
        //超时策略
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

        //熔断策略
        //如下，如果我们的业务代码连续失败50次，就触发熔断(onBreak),就不会再调用我们的业务代码，而是直接抛出BrokenCircuitException异常。
        //当熔断时间10分钟后(durationOfBreak)，切换为HalfOpen状态，触发onHalfOpen事件，此时会再调用一次我们的业务代码
        //，如果调用成功，则触发onReset事件，并解除熔断，恢复初始状态，否则立即切回熔断状态。
        var circuitBreakerPolicy = Policy.Handle<Exception>()
            .CircuitBreakerAsync
            (
                // 熔断前允许出现几次错误
                exceptionsAllowedBeforeBreaking: 50
                ,
                // 熔断时间,熔断X分钟
                durationOfBreak: TimeSpan.FromMinutes(5)
                ,
                // 熔断时触发
                onBreak: (ex, breakDelay) =>
                {
                    //todo 企业微信等通知
                    var e = ex;
                    var delay = breakDelay;
                }
                ,
                //熔断恢复时触发
                onReset: () =>
                {
                    //todo 企业微信等通知
                }
                ,
                //在熔断时间到了之后触发
                onHalfOpen: () =>
                {
                    //todo
                }
            );

        return new List<IAsyncPolicy<HttpResponseMessage>>()
        {
            retryPolicy, timeoutPolicy  //, circuitBreakerPolicy.AsAsyncPolicy<HttpResponseMessage>() // 暂时不要熔断
        };
    }

}