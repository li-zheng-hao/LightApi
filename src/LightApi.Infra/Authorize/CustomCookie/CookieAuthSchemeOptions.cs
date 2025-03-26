using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace LightApi.Infra.Authorize.CustomCookie;

public class CookieAuthSchemeOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "CookieAuth";

    public string CookieName { get; set; } = "app_auth";

    /// <summary>
    /// 过期时间
    /// </summary>
    public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromDays(30);

    public bool IsPersistent { get; set; } = true;

    public PathString Path { get; set; } = "/";

    public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;

    /// <summary>
    /// 加密密钥
    /// </summary>
    public string? SecretKey { get; set; }
}
