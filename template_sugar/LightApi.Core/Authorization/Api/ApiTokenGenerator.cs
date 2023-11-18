using System.Security.Cryptography;
using System.Text;
using EasyCaching.Core;
using LightApi.Core.Autofac;
using Microsoft.Extensions.Configuration;

namespace LightApi.Core.Authorization.Api;

public class ApiTokenGenerator:ISingletonDependency
{
    private readonly IEasyCachingProvider _cachingProvider;

    public ApiTokenGenerator(IConfiguration configuration,
        IEasyCachingProvider cachingProvider)
    {
        _cachingProvider = cachingProvider;
        RsaPublicKey = configuration["SecretKey:RsaPublicKey"] ?? RsaPublicKey;
        RsaPrivateKey = configuration["SecretKey:RsaPrivateKey"] ?? RsaPrivateKey;
    }

    #region 配置项

    private const string ClientId = "test-partner";

    private const string ClientSecret = "123456";

    private string RsaPublicKey = @"-----BEGIN RSA PUBLIC KEY-----
xxx
-----END RSA PUBLIC KEY-----";

    // 内部专用
    private string RsaPrivateKey = @"-----BEGIN RSA PRIVATE KEY-----
xxx
-----END RSA PRIVATE KEY-----";

    #endregion

    /// <summary>
    /// 添加到请求头 Authorization
    /// </summary>
    /// <returns></returns>
    public string GenerateApiToken()
    {
        string key = $"{ClientId} {ClientSecret} {DateTime.Now:yyyyMMddHHmmss} {Guid.NewGuid():N}";
        var rsa = RSA.Create();
        rsa.ImportFromPem(RsaPublicKey);
        var token = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(key), RSAEncryptionPadding.Pkcs1));
        return $"Api {token}";
    }

    /// <summary>
    ///  验证请求头 Authorization 是否合法
    /// </summary>
    /// <param name="token">Authorization头</param>
    /// <returns>解析出来的token</returns>
    public (int opCode, UserContext? userContext, string? guid)
        ResolveApiToken(string token)
    {
        if (token.StartsWith("Api"))
            token = token.Split(" ").Last();
        var rsa = RSA.Create();
        rsa.ImportFromPem(RsaPrivateKey);
        var key = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(token), RSAEncryptionPadding.Pkcs1));
        var arr = key.Split(' ');
        if (arr.Length != 4)
        {
            // throw new UnauthorizedAccessException("ApiToken格式错误");
            return (1, default, default);
        }

        var clientId = arr[0];
        var clientSecret = arr[1];
        var timestamp = arr[2];
        var guid = arr[3];
        var userContext = ValidateFromDb(clientId, clientSecret);
        
        if (!userContext.IsAuthenticated())
        {
            // throw new UnauthorizedAccessException("ApiToken验证失败");
            return (1, default, default);
        }

        var expireTime=DateTime.ParseExact(timestamp, "yyyyMMddHHmmss", null);
        // 过期时间5分钟，窗口期30秒
        if ( expireTime< DateTime.Now.AddMinutes(-5).AddSeconds(-30))
        {
            // throw new UnauthorizedAccessException("ApiToken已过期");
            return (2, default, default);
        }

        return (0, userContext, guid);
    }

    private UserContext ValidateFromDb(string clientId, string clientSecret)
    {
        return null;
        // var clientData = _cachingProvider.Get<UserContext>($"{CachingKeys.ThirdPartData}-{clientId}", () =>
        // {
        //     var user = _efRepository.AsQueryable<UserData>()
        //         .Include(it => it.UserRoles)
        //         .FirstOrDefault(it => it.UserName == clientId && it.Password == clientSecret.SHA256());
        //     if (user == null) return new UserContext();
        //     var userContext = new UserContext()
        //     {
        //         Id = user.Id,
        //         Name = user.UserName,
        //         Roles = string.Join(",", user.UserRoles.Select(it => it.Name))
        //     };
        //     return userContext;
        // }, TimeSpan.FromDays(1));
        // return clientData.Value;
    }
}