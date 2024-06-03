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
    private readonly string _encryptKey;

    public CookieDataProtector(string encryptKey)
    {
        _encryptKey = encryptKey;
    }
    public IDataProtector CreateProtector(string purpose)
    {
        return new CookieDataProtector(_encryptKey);
    }

    public byte[] Protect(byte[] plaintext)
    {
        var data=Convert.ToBase64String(plaintext);
        var encryptBase64Str = data.AESEncrypt(_encryptKey);
        var res= Convert.FromBase64String(encryptBase64Str);
        return res;
    }

    public byte[] Unprotect(byte[] protectedData)
    {
        var data = Convert.ToBase64String(protectedData);
        var decryptBase64Str = data.AESDecrypt(_encryptKey);
        var res = Convert.FromBase64String(decryptBase64Str);
        return res;
    }
}