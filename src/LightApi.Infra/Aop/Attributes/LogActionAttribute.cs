using System.Diagnostics;
using LightApi.Infra.Constant;
using LightApi.Infra.Extension;
using LightApi.Infra.Http;
using LightApi.Infra.Json;
using LightApi.Infra.OpenTelemetry;
using LightApi.Infra.Options;
using LightApi.Infra.Unify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LightApi.Infra.AOP.Attributes;

/// <summary>
/// 接口日志记录
/// </summary>
public class LogActionAttribute : ActionFilterAttribute
{
    private ILogger<LogActionAttribute> _logger;

    private IServiceProvider _svc;

    private readonly string _description;

    private readonly bool _logOnError;

    private IOptions<InfrastructureOptions> _options;
    
    /// <summary>
    /// 最大日志长度 设置为0时则使用InfrastructureOptions中的值
    /// </summary>
    public uint MaxLogLength { get; set; } = 0;
    
    /// <summary>
    /// 记录方法耗时
    /// </summary>
    /// <param name="description">日志中插入的描述内容</param>
    /// <param name="logOnError">是否仅在出现异常时打印 判断条件为http状态码等于4xx 5xx或归一化结果时UnifyResult中的success为false</param>
    public LogActionAttribute(string description = null, bool logOnError = false)
    {
        this._logOnError = logOnError;
        this._description = description;
    }

    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        _svc = actionContext.HttpContext.RequestServices;
        _options = _svc.GetRequiredService<IOptions<InfrastructureOptions>>();
        _logger = _svc.GetRequiredService<ILogger<LogActionAttribute>>();
        Activity.Current = null;
        var activity=LightApiSource.Source.StartActivity(_description.IsNullOrWhiteSpace()?actionContext.HttpContext.Request.Path:_description, ActivityKind.Server);
        if (activity != null)
        {
            Activity.Current = activity;
        }
    }

    public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
    {
        if(MaxLogLength==0) 
            MaxLogLength=_options.Value.MaxLogLength;
        if (actionExecutedContext.Exception != null)
        {
            var (paramStr, resultStr) = GetLogString(actionExecutedContext, false);

            LogError(actionExecutedContext,paramStr);
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
                    LogSuccess(actionExecutedContext,paramStr,resultStr);
                }
            }
            else if (_logOnError == false)
            {
                
                var (paramStr, resultStr) = GetLogString(actionExecutedContext);
                LogSuccess(actionExecutedContext,paramStr,resultStr);
            }
        }
        
        Activity.Current?.Stop();
        

    }

    private (string, string?) GetLogString(ActionExecutedContext actionExecutedContext,bool logResult=true)
    {
        string? resultStr = string.Empty;
        
        if (logResult)
        {
            var objectResult = actionExecutedContext.Result as ObjectResult;
            resultStr = JsonConvert.SerializeObject(objectResult==null?actionExecutedContext.Result:objectResult.Value,new JsonSerializerSettings(){ContractResolver =new DynamicContractResolver()});

        }
        
        var paramStr = JsonConvert.SerializeObject(actionExecutedContext.HttpContext.GetItem<object>(RequestContext.REQUEST_PARAMS),new JsonSerializerSettings(){ContractResolver =new DynamicContractResolver()});

        paramStr = paramStr.SafeSubString(MaxLogLength);
        resultStr = resultStr.SafeSubString(MaxLogLength);

        return (paramStr, resultStr);
    }
    
    private void LogError(ActionExecutedContext actionExecutedContext,string paramStr)
    {
        var user = actionExecutedContext.HttpContext.RequestServices.GetService<IUser>();

        _logger.LogError(actionExecutedContext.Exception,
            $"\r\n------------------------------------\r\n请求记录 \r\n描述: {_description} \r\n用户：{user?.UserName??"未登录"} \r\n请求路由: {actionExecutedContext.HttpContext.Request.Path}  \r\n请求时间: {actionExecutedContext.HttpContext.GetItem<DateTime>(RequestContext.REQUEST_BEGIN_TIME)} \r\n结束时间: {DateTime.Now} \r\n请求参数: {paramStr} \r\n出现异常: {actionExecutedContext.Exception?.Message}\r\n------------------------------------");

    }

    private void LogSuccess(ActionExecutedContext actionExecutedContext,string paramStr, string? resultStr)
    {
        var user = actionExecutedContext.HttpContext.RequestServices.GetService<IUser>();

        _logger.LogInformation(
            $"\r\n------------------------------------\r\n请求记录 \r\n描述: {_description} \r\n用户：{user?.UserName??"未登录"} \r\n请求路由: {actionExecutedContext.HttpContext.Request.Path}  \r\n请求时间: {actionExecutedContext.HttpContext.GetItem<DateTime>(RequestContext.REQUEST_BEGIN_TIME)} \r\n结束时间: {DateTime.Now} \r\n请求参数: {paramStr} \r\n返回结果: {resultStr} \r\n------------------------------------");

    }
}