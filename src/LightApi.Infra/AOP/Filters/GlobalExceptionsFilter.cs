using System.Text;
using LightApi.Infra.Constant;
using LightApi.Infra.Extension;
using LightApi.Infra.Http;
using LightApi.Infra.InfraException;
using LightApi.Infra.Json;
using LightApi.Infra.Options;
using LightApi.Infra.Unify;
using Masuit.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ExceptionContext = Microsoft.AspNetCore.Mvc.Filters.ExceptionContext;
using IExceptionFilter = Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter;

namespace LightApi.Infra.AOP.Filters;

/// <summary>
/// 接口全局异常错误日志
/// </summary>
public class GlobalExceptionsFilter : IExceptionFilter, IActionFilter
{
    private readonly ILogger<GlobalExceptionsFilter> _logger;

    private readonly IOptions<InfrastructureOptions> _options;

    private readonly IUnifyResultProvider _unifyResultProvider;

    public GlobalExceptionsFilter(ILogger<GlobalExceptionsFilter> logger, IOptions<InfrastructureOptions> options,
        IUnifyResultProvider unifyResultProvider)
    {
        _logger = logger;
        _options = options;
        _unifyResultProvider = unifyResultProvider;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BusinessException customException)
        {
            if (customException.Code == -1) customException.Code = _options.Value.DefaultFailureBusinessExceptionCode;

            var jm = _unifyResultProvider.Failure(customException.Body, customException.Code,
                customException.Message);

            if (customException.HttpStatusCode.HasValue)
                context.HttpContext.Response.StatusCode = (int)customException.HttpStatusCode.Value;

            context.ExceptionHandled = true;

            context.Result = new JsonResult(jm);
        }
        else
        {
            var paramString = GetLogString(context.HttpContext);

            LogError(context, paramString);

            //处理各种异常
            var jm = _unifyResultProvider.Failure(null, _options.Value.UnCatchExceptionBusinessCode,
                _options.Value.DefaultErrorMessage);

            context.HttpContext.Response.StatusCode = (int)_options.Value.DefaultUnCatchErrorHttpStatusCode;

            context.ExceptionHandled = true;

            context.Result = new JsonResult(jm);
        }
    }


    private string GetLogString(HttpContext context)
    {
        context.Items.TryGetValue(RequestContext.REQUEST_PARAMS, out var requestParams);
        
        if (requestParams == null)
            return string.Empty;

        var paramStr = JsonConvert.SerializeObject(requestParams,
            new JsonSerializerSettings() { ContractResolver = new DynamicContractResolver() });

        paramStr = paramStr.SafeSubString(_options.Value.MaxLogLength);

        return paramStr;
    }

    private void LogError(ExceptionContext context, string requestBody)
    {
        var user = context.HttpContext.RequestServices.GetService<IUser>();

        _logger.LogError(context.Exception,
            $"\r\n------------------------------------\r\n未捕获异常  \r\n用户：{user?.UserName ?? "未登录"} \r\n请求路由: {context.HttpContext.Request.Path} \r\n请求时间: {context.HttpContext.GetItem<string>(RequestContext.REQUEST_BEGIN_TIME)} \r\n请求参数: {requestBody} \r\n出现异常: {context.Exception?.Message}\r\n------------------------------------");
    }


    public void OnActionExecuting(ActionExecutingContext context)
    {
        // 添加请求上下文数据
        context.HttpContext.Items.TryAdd(RequestContext.REQUEST_PARAMS, context.ActionArguments);
        context.HttpContext.Items.TryAdd(RequestContext.REQUEST_BEGIN_TIME, DateTime.Now);
        context.HttpContext.Items.TryAdd(RequestContext.REQUEST_ID, Guid.NewGuid().ToString());
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}