using FB.Infrastructure.Extension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FB.Infrastructure.AOP
{
    public class TimerFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 慢查询时间 单位毫秒
        /// </summary>
        public uint? SlowQueryMilliseconds { get; set; }

        private ILogger<TimerFilterAttribute> _logger;

        private IDictionary<string, object?> _args;

        private IOptions<InfrastructureOptions> _options;

        private DateTime beginTime { get; set; }



        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            _args = actionContext.ActionArguments;

            beginTime = DateTime.Now;
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            _options = actionExecutedContext.HttpContext.RequestServices.GetService<IOptions<InfrastructureOptions>>()!;

            if (_options.Value.IgnoreTimeFilterUrls.Any(it => actionExecutedContext.HttpContext.Request.Path.Value?.Contains(it) ?? false))
                return;

            SlowQueryMilliseconds??=_options.Value.SlowQueryMilliseconds;
            
            _logger = actionExecutedContext.HttpContext.RequestServices.GetService<ILogger<TimerFilterAttribute>>()!;

            var endTime = DateTime.Now;

            var span = endTime - beginTime;

            if (span.TotalMilliseconds > SlowQueryMilliseconds)
            {
                var argsAndResults = GetLogString(actionExecutedContext);
                
                _logger.LogWarning("慢查询 请求的URI为:{Path}{RequestQueryString} 耗时: {SpanTotalMilliseconds}毫秒 开始时间{dateTime} 结束时间{endTime} 请求参数: {paramStr} 返回结果: {resultStr}",
                    actionExecutedContext.HttpContext.Request.Path,
                    actionExecutedContext.HttpContext.Request.QueryString, span.TotalMilliseconds, beginTime, endTime,argsAndResults.Item1,argsAndResults.Item2);
            }
        }
        
        private (string, string) GetLogString(ActionExecutedContext actionExecutedContext)
        {
            var paramStr = JsonConvert.SerializeObject(_args);
            var resultStr = JsonConvert.SerializeObject(actionExecutedContext.Result);

            paramStr = paramStr.SafeSubString(_options.Value.MaxLogLength);
            resultStr = resultStr.SafeSubString(_options.Value.MaxLogLength);

            return (paramStr, resultStr);
        }
    }
}