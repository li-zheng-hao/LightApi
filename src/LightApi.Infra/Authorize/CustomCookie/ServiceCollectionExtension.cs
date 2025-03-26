using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightApi.Infra.Authorize.CustomCookie;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 添加自定义Cookie认证 使用AES加解密
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomCookieAuth(
        this IServiceCollection services,
        Action<CookieAuthSchemeOptions> setupAction
    )
    {
        services.TryAddSingleton<CookieGenerator>();
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthSchemeOptions.SchemeName;
            })
            .AddScheme<CookieAuthSchemeOptions, CookieAuthHandler>(
                CookieAuthSchemeOptions.SchemeName,
                setupAction
            );
        return services;
    }
}
