using FB.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LightApi.Core.Aop;

public class ExceptionGuardAttribute : ActionFilterAttribute
{
    private readonly string[] _messageKeys;

    public Type[]? ExceptionTypes { get; set; }

    public BusinessErrorCode ErrorCode { get; set; } = BusinessErrorCode.Code1002;
    
    /// <summary>
    /// 填充到业务消息中的参数
    /// </summary>
    public string[]? MessageParameter { get; set; }
    /// <summary>
    /// 捕获包含特定消息的异常并转换成业务异常
    /// </summary>
    /// <param name="messageKeys">小写</param>
    public ExceptionGuardAttribute(params string[] messageKeys)
    {
        _messageKeys = messageKeys;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception == null || context.Exception is BusinessException) return;

        // 捕获所有异常
        if (_messageKeys.Length == 0 && ExceptionTypes.Length == 0)
        {
            if(MessageParameter?.Any() is true)
                throw ErrorCode.ToBusinessException(MessageParameter);
            throw ErrorCode.ToBusinessException();
        }
        // 比较Message key

        if (_messageKeys.Length > 0 && _messageKeys.Any(it =>
                context.Exception.Message.ToLower().Contains(it) ||
                context.Exception?.InnerException?.Message.ToLower().Contains(it) is true))
        {
            if(MessageParameter?.Any() is true)
                throw ErrorCode.ToBusinessException(MessageParameter);
            throw ErrorCode.ToBusinessException();
        }
        // 比较错误类型

        if (ExceptionTypes?.Length > 0&&ExceptionTypes.Any(it => it.IsInstanceOfType(context.Exception)||it.IsInstanceOfType(context.Exception?.InnerException)))
        {
            if(MessageParameter?.Any() is true)
                throw ErrorCode.ToBusinessException(MessageParameter);
            throw ErrorCode.ToBusinessException();
        }

    }
}