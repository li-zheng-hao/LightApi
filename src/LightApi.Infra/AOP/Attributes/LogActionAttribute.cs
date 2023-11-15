using LightApi.Infra.Extension;
using LightApi.Infra.Json;
using LightApi.Infra.Options;
using LightApi.Infra.Unify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LightApi.Infra.AOP.Attributes;

public class LogActionAttribute : ActionFilterAttribute
{
    private ILogger<LogActionAttribute> _logger;

    private IServiceProvider _svc;

    private readonly string _description;

    private IDictionary<string, object?> _args;

    private readonly bool _logOnError;

    private IOptions<InfrastructureOptions> _options;

    
    /// <summary>
    /// 记录方法耗时
    /// </summary>
    /// <param name="description">日志中插入的描述内容</param>
    /// <param name="logOnError">是否仅在出现异常时打印 判断条件为http状态码等于4xx 5xx或归一化结果时UnifyResult中的success为false</param>
    public LogActionAttribute(string description = null, bool logOnError = true)
    {
        this._logOnError = logOnError;
        this._description = description;
    }

    public DateTime beginTime { get; set; }

    /// <summary>
    /// 最大日志长度 不设置则使用InfrastructureOptions中的值
    /// </summary>
    public uint MaxLogLength { get; set; } = 0;

    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        _svc = actionContext.HttpContext.RequestServices;
        _options = _svc.GetRequiredService<IOptions<InfrastructureOptions>>();
        _logger = _svc.GetService<ILogger<LogActionAttribute>>();
        _args = actionContext.ActionArguments;
        beginTime = DateTime.Now;
    }

    public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
    {
        if(MaxLogLength==0) 
            MaxLogLength=_options.Value.MaxLogLength;
        
        if (actionExecutedContext.Exception != null)
        {
            var (paramStr, resultStr) = GetLogString(actionExecutedContext);

            _logger.LogInformation(
                $"请求记录 描述: {_description} 请求路由: {actionExecutedContext.HttpContext.Request.Path}  请求时间: {beginTime} 结束时间: {DateTime.Now} 请求参数: {paramStr} 出现异常: {actionExecutedContext.Exception}");
        }
        else
        {
            if (_logOnError)
            {
                var objectResult = actionExecutedContext.Result as ObjectResult;
                var resp = objectResult?.Value as UnifyResult;

                if (resp?.success == false || actionExecutedContext.Exception != null)
                {
                    var (paramStr, resultStr) = GetLogString(actionExecutedContext);

                    _logger.LogInformation(
                        $"请求记录 描述: {_description} 请求路由: {actionExecutedContext.HttpContext.Request.Path}  请求时间: {beginTime} 结束时间: {DateTime.Now} 请求参数: {paramStr} 返回结果: {resultStr}");
                }
            }
            else if (_logOnError == false)
            {
                var (paramStr, resultStr) = GetLogString(actionExecutedContext);

                _logger.LogInformation(
                    $"请求记录 描述: {_description} 请求路由: {actionExecutedContext.HttpContext.Request.Path}  请求时间: {beginTime} 结束时间: {DateTime.Now} 请求参数: {paramStr} 返回结果: {resultStr}");
            }
        }
    }

    private (string, string) GetLogString(ActionExecutedContext actionExecutedContext)
    {
        var paramStr = JsonConvert.SerializeObject(_args,new JsonSerializerSettings(){ContractResolver =new DynamicContractResolver()});
        var resultStr = JsonConvert.SerializeObject(actionExecutedContext.Result,new JsonSerializerSettings(){ContractResolver =new DynamicContractResolver()});

        paramStr = paramStr.SafeSubString(MaxLogLength);
        resultStr = resultStr.SafeSubString(MaxLogLength);

        return (paramStr, resultStr);
    }
}