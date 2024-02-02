using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace LightApi.Core.Authorization.Hybrid;

public class CustomHybridAuthHandler : AuthenticationHandler<CustomHybridAuthSchemeOptions>
{
    public CustomHybridAuthHandler(IOptionsMonitor<CustomHybridAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
#if DEBUG
        // if (Request.Headers["Referer"].ToString().Contains("swagger")&&string.IsNullOrWhiteSpace(Request.Headers.Authorization.ToString()))
        // {
        //     var claimsIdentity = new ClaimsIdentity(default,
        //         nameof(CustomAuthorizationSchemes.JwtSchemeName));
        //
        //     var ticket = new AuthenticationTicket(
        //         new ClaimsPrincipal(claimsIdentity), Scheme.Name);
        //     return Task.FromResult(AuthenticateResult.Success(ticket));
        //
        // }
#endif
        if (Request.Cookies.ContainsKey("LightApi"))
        {
            return Context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        
        
        var header = Request.Headers[HeaderNames.Authorization].ToString();

        if (header.StartsWith(CustomAuthorizationSchemes.ApiSchemeName, StringComparison.OrdinalIgnoreCase))
        {
            return Context.AuthenticateAsync(CustomAuthorizationSchemes.ApiSchemeName);
        }

        if(header.StartsWith(CustomAuthorizationSchemes.JwtSchemeName, StringComparison.OrdinalIgnoreCase))
        {
            return Context.AuthenticateAsync(CustomAuthorizationSchemes.JwtSchemeName);
        }
        return Context.AuthenticateAsync(CustomAuthorizationSchemes.JwtSchemeName);

        // return Task.FromResult(AuthenticateResult.Fail("Authorization header not found or not supported."));
    }

    protected async override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        await base.HandleChallengeAsync(properties);
        if (Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            // 自定义响应
            // Response.Headers.Add("WWW-Authenticate", "");
            Response.ContentType = "application/json";
            await WriteProblemDetailsAsync();
        }
        
    }
    private Task WriteProblemDetailsAsync()
    {
        // var result = new JsonResult(new UnifyResult()
        // {
        //     success = false,
        //     code =  401,
        //     msg = BusinessErrorCode.Code401.GetDescription()
        // });
        //
        // var executor = Context.RequestServices.GetRequiredService<IActionResultExecutor<JsonResult>>();
        // var routeData = Context.GetRouteData() ?? new RouteData();
        // var actionContext = new ActionContext(Context, routeData, new ActionDescriptor());
        // return executor.ExecuteAsync(actionContext, result);
        return Task.CompletedTask;
    }
}