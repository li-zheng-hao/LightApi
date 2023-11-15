using LightApi.Infra.Options;

namespace LightApi.Infra.InfraException;

/// <summary>
/// 自定义抛出的业务异常，不会被全局过滤器记录到日志平台
/// </summary>
public class BusinessException : Exception
{
    public int Code { get; set; }

    public object Body { get; set; }

    /// <summary>
    /// 业务错误  如果code为-1，则使用InfrastructureOptions.DefaultFailureBusinessException
    /// </summary>
    /// <see cref="InfrastructureOptions"/>
    /// <param name="msg"></param>
    /// <param name="code"></param>
    public BusinessException(string msg, int code = -1) : base(msg)
    {
        this.Code = code;
    }

    /// <summary>
    /// 业务错误  如果code为-1，则使用InfrastructureOptions.DefaultFailureBusinessException
    /// </summary>
    /// <see cref="InfrastructureOptions"/>
    /// <param name="msg"></param>
    /// <param name="body"></param>
    /// <param name="code"></param>
    public BusinessException(string msg, object body,int code = -1) : base(msg)
    {
        this.Code = code;
        this.Body = body;
    }
}