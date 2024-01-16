using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LightApi.Infra.Constant;

public class RequestContext
{
    /// <summary>
    /// 统一前缀
    /// </summary>
    public const string Prefix = "LIGHTAPI_";
    /// <summary>
    /// 请求开始时间
    /// </summary>
    public const string REQUEST_BEGIN_TIME = Prefix + nameof(REQUEST_BEGIN_TIME);
    
    /// <summary>
    /// 请求唯一ID
    /// </summary>
    public const string REQUEST_ID = Prefix + nameof(REQUEST_ID);
    
    /// <summary>
    /// 请求参数
    /// </summary>
    public const string REQUEST_PARAMS= Prefix + nameof(REQUEST_PARAMS);
}