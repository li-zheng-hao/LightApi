using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using LightApi.Infra.Autofac;
using Masuit.Tools;
using Masuit.Tools.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace LightApi.Infra.Authorize.CustomCookie;

public class CookieGenerator : ISingletonDependency
{
    private readonly string? _secretKey;

    private CookieAuthSchemeOptions _options;

    public CookieGenerator(IOptionsMonitor<CookieAuthSchemeOptions> options)
    {
        _options = options.Get(CookieAuthSchemeOptions.SchemeName);
        string? existKey = _options.SecretKey;
        if (existKey == null)
            Log.Warning("配置文件未读取到对称加密密钥,使用默认密钥,需要在配置文件中配置SecretKey:JwtSecretKey!");
        _secretKey = existKey ?? "no_secret_key_this_is_a_default_key";
    }

    /// <summary>
    /// 生成Cookie内容
    /// </summary>
    /// <returns></returns>
    public string CreateCookie(string userId)
    {
        var cookie = new CookiePayload()
        {
            UserId = userId,
            ExpireTimeStamp = DateTimeOffset
                .UtcNow.Add(_options.ExpireTimeSpan)
                .ToUnixTimeSeconds(),
        };
        var content = cookie.ToJsonString().AESEncrypt(_secretKey);
        return content;
    }

    /// <summary>
    /// 解析Cookie内容
    /// </summary>
    /// <param name="cookie">加密的cookie字符串</param>
    /// <returns>(状态码, 解析后的payload) 0:成功,1:过期,2:解密失败</returns>
    public (int Status, CookiePayload? payload) ResolveCookie(string cookie)
    {
        try
        {
            // 解密cookie
            var decryptContent = cookie.AESDecrypt(_secretKey);
            var payload = JsonConvert.DeserializeObject<CookiePayload>(decryptContent);

            if (payload == null)
                return (2, null);

            // 检查是否过期
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (currentTimestamp > payload.ExpireTimeStamp)
                return (1, payload);

            return (0, payload);
        }
        catch
        {
            return (2, null);
        }
    }

    public void AddCookie(HttpContext context, string userId)
    {
        var cookie = CreateCookie(userId);
        context.Response.Cookies.Append(
            _options.CookieName,
            cookie,
            new CookieOptions()
            {
                MaxAge = _options.ExpireTimeSpan,
                Path = _options.Path,
                HttpOnly = true,
                SameSite = _options.SameSite,
            }
        );
    }
}
