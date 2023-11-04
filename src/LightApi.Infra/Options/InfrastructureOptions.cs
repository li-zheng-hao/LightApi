using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FB.Infrastructure;

/// <summary>
/// 基础设施配置
/// </summary>
public class InfrastructureOptions
{
    /// <summary>
    /// 默认成功时的业务码
    /// </summary>
    public int DefaultSuccessCode { get; set; } = 200;

    /// <summary>
    /// 默认成功时的消息
    /// </summary>
    public string DefaultSuccessMessage { get; set; } = "";

    /// <summary>
    /// 默认的错误返回信息
    /// </summary>
    public string DefaultErrorMessage { get; set; } = "系统繁忙，请稍后再试";


    /// <summary>
    /// 默认的模型验证错误码
    /// </summary>
    public HttpStatusCode DefaultModelValidateErrorHttpStatusCode { get; set; } = HttpStatusCode.BadRequest;

    /// <summary>
    /// 默认的模型验证业务错误消息
    /// </summary>
    public string DefaultModelValidateErrorMessage { get; set; } = "参数非法";

    /// <summary>
    /// 默认的模型验证业务错误码
    /// </summary>
    public int DefaultModelValidateErrorBusinessCode { get; set; } = 400;


    /// <summary>
    /// 未捕获异常的错误业务码
    /// </summary>
    public int UnCatchExceptionCode { get; set; } = 888;

    /// <summary>
    /// 是否开启全局权限校验,开启前需要确认已经注册了框架提供的权限校验服务
    /// </summary>
    public bool EnableGlobalAuthorize { get; set; }

    /// <summary>
    /// 是否压制非空引用类型的隐式必需属性
    /// </summary>
    public bool SuppressImplicitRequiredAttributeForNonNullableReferenceTypes { get; set; } = true;

    /// <summary>
    /// 是否开启全局异常过滤器 默认true
    /// </summary>
    public bool EnableGlobalExceptionFilter { get; set; } = true;

    /// <summary>
    /// 是否开启全局模型验证过滤器 默认true
    /// </summary>
    public bool EnableGlobalModelValidator { get; set; } = true;

    /// <summary>
    /// 是否开启全局Http响应统一包装 默认false
    /// </summary>
    public bool EnableGlobalUnifyResult { get; set; } = false;

    /// <summary>
    /// 系统中各种日志的最大长度，超过长度后会自动截取 默认2000
    /// </summary>
    public uint MaxLogLength { get; set; } = 2000;

    /// <summary>
    /// TimeFilter过滤器需要忽略的Url 例如 /api/message 会忽略所有以/api/message开头的Url
    /// </summary>
    public List<string> IgnoreTimeFilterUrls { get; set; } = new List<string>();

    /// <summary>
    /// 慢查询的最小时间 默认1000ms
    /// </summary>
    public uint? SlowQueryMilliseconds { get; set; } = 1000;
}