namespace LightApi.Infra.Authorize.CustomCookie;

public class CookiePayload
{
    /// <summary>
    /// 过期时间戳 utc
    /// </summary>
    public long ExpireTimeStamp { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public string UserId { get; set; }
}
