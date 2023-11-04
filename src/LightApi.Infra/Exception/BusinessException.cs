using System.Net;

namespace FB.Infrastructure.Core;

/// <summary>
/// 自定义抛出的业务异常，不会被全局过滤器记录到日志平台
/// </summary>
public class BusinessException : Exception
{
    public int Code { get; set; }

    public object Body { get; set; }
    /// <summary>
    /// 是否需要改变Http响应头的状态码
    /// </summary>
    public HttpStatusCode? HttpStatusCode { get; set; }

    public BusinessException(string msg, int code = 888,
        HttpStatusCode statusCode = System.Net.HttpStatusCode.BadRequest) : base(msg)
    {
        this.HttpStatusCode = statusCode;
        this.Code = code;
    }
    public BusinessException(string msg, object body,int code = 888,
        HttpStatusCode statusCode = System.Net.HttpStatusCode.BadRequest) : base(msg)
    {
        this.HttpStatusCode = statusCode;
        this.Code = code;
        this.Body = body;
    }
}