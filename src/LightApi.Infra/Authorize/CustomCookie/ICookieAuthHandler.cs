using LightApi.Infra.Http;

namespace LightApi.Infra.Authorize.CustomCookie;

public interface ICookieAuthHandler
{
    /// <summary>
    /// 验证用户 已验证并填充userId
    /// </summary>
    Task AuthenticateAsync(IUser user);
}
