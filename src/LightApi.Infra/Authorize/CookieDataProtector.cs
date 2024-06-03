using LightApi.Infra.Extension;
using LightApi.Infra.Options;
using Masuit.Tools.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace LightApi.Infra.Authorize;

/// <summary>
/// cookie自定义加密
/// </summary>
public class CookieDataProtector : IDataProtector
{
    private readonly IOptions<InfrastructureOptions> _options;

    public CookieDataProtector(IOptions<InfrastructureOptions> options)
    {
        _options = options;
        if(_options.Value.EncryptionKey.IsNullOrWhiteSpace()) throw new Exception("加密秘钥不能为空");
    }
    public IDataProtector CreateProtector(string purpose)
    {
        return new CookieDataProtector(_options);
    }

    public byte[] Protect(byte[] plaintext)
    {
        var data=Base64UrlTextEncoder.Encode(plaintext);
        var encryptBase64Str = data.AESEncrypt(_options.Value.EncryptionKey);
        return Base64UrlTextEncoder.Decode(encryptBase64Str);
    }

    public byte[] Unprotect(byte[] protectedData)
    {
        var data=Base64UrlTextEncoder.Encode(protectedData);
        var encryptBase64Str = data.AESDecrypt(_options.Value.EncryptionKey);
        return Base64UrlTextEncoder.Decode(encryptBase64Str);
    }
}