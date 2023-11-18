using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using LightApi.Core.Autofac;
using Masuit.Tools;
using Masuit.Tools.Security;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace LightApi.Core.Authorization.Jwt;

public class JwtTokenGenerator:ISingletonDependency
{
    private readonly string? _jwtSecret;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        var existKey=configuration["SecretKey:JwtSecretKey"];
        _jwtSecret = existKey ?? "no_jwt_secret_key_this_is_a_default_key";
        if(existKey==null)
            Log.Warning("配置文件未读取到jwt密钥,使用默认密钥,需要在配置文件中配置SecretKey:JwtSecretKey!");
    }

    /// <summary>
    /// 对称加密Token
    /// </summary>
    /// <param name="model"></param>
    /// <param name="expireSeconds">过期时间,单位秒,默认30分钟</param>
    /// <returns></returns>
    public string CreateToken(UserContext? model = null, int expireSeconds = 60 * 30)
    {
        var expireTime=DateTime.Now.AddSeconds(expireSeconds);
        var claims = model.ToDictionary();
        var token = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .AddClaims(claims)
            .ExpirationTime(expireTime)
            .WithSecret(_jwtSecret) // 这个secret是关键，要和上面加密的secret一致
            .Encode();

        return token;
    }

    /// <summary>
    /// 对称加密Token
    /// </summary>
    /// <param name="model"></param>
    /// <param name="expireSeconds">过期时间,单位秒,默认30分钟</param>
    /// <returns></returns>
    public string CreateRefreshToken(int userId, int expireSeconds = 60 * 30)
    {
        string token = $"{userId} {DateTime.Now.AddSeconds(expireSeconds).Ticks}".DesEncrypt(_jwtSecret);
        return token;
    }

    /// <summary>
    ///  对称解密
    /// <exception cref="TokenExpiredException">token超时</exception>
    /// <exception cref="TokenNotYetValidException">token无效</exception>
    /// <exception cref="InvalidTokenPartsException">token无效</exception>
    /// </summary>
    public UserContext ResolveToken(string token)
    {
        var validationParameters = ValidationParameters.Default;
        validationParameters.ValidateExpirationTime = true;
        validationParameters.TimeMargin = 30;
        
        var json = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .MustVerifySignature()
            .WithValidationParameters(validationParameters)
            .WithSecret(_jwtSecret) // 这个secret是关键，要和上面加密的secret一致
            .Decode(token);
        var model = JsonConvert.DeserializeObject<UserContext>(json);

        return model;
    }

    /// <summary>
    /// 验证token是否有效
    /// </summary>
    /// <param name="token"></param>
    /// <param name="delaySeconds">默认过渡期15秒</param>
    /// <returns>0 通过 1超时 2其他错误</returns>
    public (int code,UserContext context) ValidateToken(string token)
    {
        try
        {
            var model = ResolveToken(token);
            return (0, model);
        }
        catch (TokenExpiredException)
        {
            return (1, default);
        }
        catch (Exception)
        {
            return (2,default);
        }
    }

    /// <summary>
    /// 验证refresh_token是否有效
    /// </summary>
    /// <param name="token"></param>
    /// <param name="expireTime">过期时间 相对DateTime.Now来说 不填则默认0</param>
    /// <param name="delaySeconds">默认过渡期15秒</param>
    /// <returns>0 通过 1超时 2其他错误</returns>
    public (int code, int userId) ValidateRefreshToken(string token, TimeSpan? expireTime=null,int delaySeconds = 15)
    {
        try
        {
            var str = token.DesDecrypt(_jwtSecret);
            var arr = str.Split(" ");
            if (arr.Length != 2)
                return (2, default);
            var userId = int.Parse(arr[0]);
            var exp = long.Parse(arr[1]);
            if (exp < DateTime.Now.Subtract(expireTime??TimeSpan.Zero).AddSeconds(-delaySeconds).Ticks)
                return (1, default);
            return (0, userId);
        }
        catch (Exception)
        {
            return (2, default);
        }
    }

    /// <summary>
    /// 解析token 不验证
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Dictionary<string, object>? ResolveTokenWithoutValidate(string token)
    {
        var json = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .DoNotVerifySignature()
            .Decode(token);
        var model = JsonConvert.DeserializeObject<Dictionary<string,object>>(json);

        return model;
    }
}