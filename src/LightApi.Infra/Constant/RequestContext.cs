using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LightApi.Infra.Constant;

public class RequestContext
{
    /// <summary>
    /// 统一前缀
    /// </summary>
    public static string REQUEST_PREFIX = "LIGHTAPI_";
    /// <summary>
    /// 请求开始时间
    /// </summary>
    public static string REQUEST_BEGIN_TIME = REQUEST_PREFIX + nameof(REQUEST_BEGIN_TIME);
    
    /// <summary>
    /// 请求唯一ID
    /// </summary>
    public static string REQUEST_ID = REQUEST_PREFIX + nameof(REQUEST_ID);
    
    /// <summary>
    /// 请求参数
    /// </summary>
    public static string REQUEST_PARAMS= REQUEST_PREFIX + nameof(REQUEST_PARAMS);
}