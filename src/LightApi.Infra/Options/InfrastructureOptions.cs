﻿using System.Net;

namespace LightApi.Infra.Options;

/// <summary>
/// 基础设施配置
/// </summary>
public class InfrastructureOptions
{
    #region 状态码相关
    /// <summary>
    /// 默认成功时的业务码
    /// </summary>
    public int DefaultSuccessBusinessCode { get; set; } = 200;

    /// <summary>
    /// 默认成功时的消息
    /// </summary>
    public string DefaultSuccessMessage { get; set; } = "";

    /// <summary>
    /// 默认的模型验证业务错误码
    /// </summary>
    public int DefaultModelValidateErrorBusinessCode { get; set; } = 400;

    /// <summary>
    /// 未捕获异常的错误业务码
    /// </summary>
    public int UnCatchExceptionBusinessCode { get; set; } = 888;

    /// <summary>
    /// 默认业务异常错误码
    /// </summary>
    public int DefaultFailureBusinessExceptionCode { get; set; } = 400;

    /// <summary>
    /// 默认的未捕获内部异常时HTTP错误码
    /// </summary>
    public HttpStatusCode DefaultUnCatchErrorHttpStatusCode { get; set; } =
        HttpStatusCode.InternalServerError;

    /// <summary>
    /// 默认的模型验证HTTP错误码
    /// </summary>
    public HttpStatusCode DefaultModelValidateErrorHttpStatusCode { get; set; } = HttpStatusCode.OK;

    #endregion

    /// <summary>
    /// 默认的错误返回信息
    /// </summary>
    public string DefaultErrorMessage { get; set; } = "系统繁忙，请稍后再试";

    /// <summary>
    /// 默认的模型验证业务错误消息
    /// </summary>
    public string DefaultModelValidateErrorMessage { get; set; } = "参数非法";

    /// <summary>
    /// 将第一个模型验证错误消息作为业务错误消息
    /// </summary>
    public bool UseFirstModelValidateErrorMessage { get; set; } = false;

    /// <summary>
    /// 如果有多条错误消息，将错误消息数组放到extraInfo部分返回
    /// </summary>
    public bool ReturnModelValidateErrorMessageInExtraInfo { get; set; } = false;

    /// <summary>
    /// 是否在出现未处理异常时接口附加错误信息
    /// </summary>
    public bool IncludeUnCatchExceptionTraceInfo { get; set; } = true;

    /// <summary>
    /// 在<see cref="IncludeUnCatchExceptionTraceInfo"/>为true的前提下，是否需要包含错误的堆栈信息，在生产环境下建议关闭，否则可能会泄露服务器信息
    /// </summary>
    public bool IncludeExceptionStack { get; set; } = true;

    /// <summary>
    /// 是否开启全局权限校验
    /// </summary>
    public bool EnableGlobalAuthorize { get; set; } = false;

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
    /// 是否开启全局Http响应统一包装 默认true
    /// </summary>
    public bool EnableGlobalUnifyResult { get; set; } = true;

    /// <summary>
    /// 系统中各种日志的最大长度，超过长度后会自动截取 默认8000
    /// </summary>
    public uint MaxLogLength { get; set; } = 8000;

    /// <summary>
    /// 对称加密秘钥
    /// </summary>
    public string? EncryptionKey { get; set; }
}
