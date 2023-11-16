using System.Text;
using LightApi.Infra.Extension;
using LightApi.Infra.InfraException;
using LightApi.Infra.Options;
using LightApi.Infra.Unify;
using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ExceptionContext = Microsoft.AspNetCore.Mvc.Filters.ExceptionContext;
using IExceptionFilter = Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter;

namespace LightApi.Infra.AOP.Filters;

/// <summary>
/// 接口全局异常错误日志
/// </summary>
public class GlobalExceptionsFilter : IExceptionFilter
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
        if (context.Exception is BusinessException)
        {
            var customException = (context.Exception as BusinessException);

            if (customException.Code == -1) customException.Code = _options.Value.DefaultFailureBusinessExceptionCode;
            
            var jm = _unifyResultProvider.Failure(customException.Body, customException.Code,
                context.Exception.Message);

            context.ExceptionHandled = true;

            context.Result = new JsonResult(jm);
        }
        else
        {
            RequestInfo requestInfo = new();

            // IMPORTANT: Leave the body open so the next middleware can read it.
            using (var reader = new StreamReader(
                       context.HttpContext.Request.Body,
                       Encoding.UTF8))
            {
                if (context.HttpContext.Request.Body.CanSeek)
                    context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                requestInfo.Body = reader.ReadToEndAsync().Result;

                requestInfo.Body = requestInfo.Body.SafeSubString(_options.Value.MaxLogLength);

                if (context.HttpContext.Request.Body.CanSeek)
                    context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }


            requestInfo.Path = $"{context.HttpContext.Request.Path}/{context.HttpContext.Request.QueryString}";

            //处理各种异常
            var jm = _unifyResultProvider.Failure(null, _options.Value.UnCatchExceptionBusinessCode,
                _options.Value.DefaultErrorMessage);

            context.HttpContext.Response.StatusCode = (int)_options.Value.DefaultUnCatchErrorHttpStatusCode;

            context.ExceptionHandled = true;
            context.Result = new JsonResult(jm);
            _logger.LogError(context.Exception, "全局未捕获异常 Http信息 {Json}", requestInfo.ToJsonString());
        }
    }
}

internal class RequestInfo
{
    public string Path { get; set; }

    public string Body { get; set; }
}