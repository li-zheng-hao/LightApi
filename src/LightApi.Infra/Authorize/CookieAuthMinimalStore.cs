using System.Security.Claims;
using LightApi.Infra.Extension;
using LightApi.Infra.Options;
using Masuit.Tools;
using Masuit.Tools.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LightApi.Infra.Authorize;

public class CookieAuthMinimalStore:ITicketStore
{
    private readonly IOptions<InfrastructureOptions> _options;

    public CookieAuthMinimalStore(IOptions<InfrastructureOptions> options)
    {
        _options = options;
        if(_options.Value.EncryptionKey.IsNullOrWhiteSpace()) throw new Exception("加密秘钥不能为空");
    }
    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        Dictionary<string, string> payload = new();
        
        foreach (var principalClaim in ticket.Principal.Claims)
        {
            payload.Add(principalClaim.Type, principalClaim.Value);
        }

       
        return Task.FromResult(payload.ToJsonString().AESEncrypt(_options.Value.EncryptionKey));
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        return Task.CompletedTask;
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var payload = JsonConvert.DeserializeObject<Dictionary<string,string>>( key.AESDecrypt(_options.Value.EncryptionKey));
        var claims = payload!.Select(it => new Claim(it.Key, it.Value)).ToList();
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
        return Task.FromResult<AuthenticationTicket?>(ticket);
    }

    public Task RemoveAsync(string key)
    {
        return Task.CompletedTask;
    }
}

