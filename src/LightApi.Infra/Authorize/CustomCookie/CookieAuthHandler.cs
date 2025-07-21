using System.Security.Claims;
using System.Text.Encodings.Web;
using LightApi.Infra.Http;
using LightApi.Infra.Unify;
using Mapster;
using Masuit.Tools;
using Masuit.Tools.Systems;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace LightApi.Infra.Authorize.CustomCookie;

public class CookieAuthHandler
    : AuthenticationHandler<CookieAuthSchemeOptions>,
        IAuthenticationSignOutHandler
{
    private readonly ICookieAuthHandler? _authHandler;

    public CookieAuthHandler(
        IOptionsMonitor<CookieAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ICookieAuthHandler? authHandler = null
    )
        : base(options, logger, encoder, clock)
    {
        _authHandler = authHandler;
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        Response.Cookies.Delete(Options.CookieName);
        return Task.CompletedTask;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var cookie = Request.Cookies[Options.CookieName];

        if (string.IsNullOrEmpty(cookie))
        {
            return AuthenticateResult.Fail("Authorize failed");
        }

        var cookieGenerator = Context.RequestServices.GetRequiredService<CookieGenerator>();

        try
        {
            var result = cookieGenerator.ResolveCookie(cookie);

            if (result.Status != 0)
                return AuthenticateResult.Fail("Authorize failed");

            var tokenModel = result.payload;

            var userContext = Context.RequestServices.GetRequiredService<IUser>();

            userContext.Id = tokenModel!.UserId;

            tokenModel.Adapt(userContext);

            if (_authHandler != null)
            {
                await _authHandler.AuthenticateAsync(userContext);
            }
            var claims = new[] { new Claim("Id", userContext!.Id!), };

            var claimsIdentity = new ClaimsIdentity(claims, nameof(CookieAuthHandler));

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

            // 少于一半过期时间，刷新cookie
            if (
                (result.payload!.ExpireTimeStamp - DateTimeOffset.Now.ToUnixTimeSeconds())
                <= Math.Floor(Options.ExpireTimeSpan.TotalSeconds / 2.0d)
            )
            {
                var newCookie = cookieGenerator.CreateCookie(tokenModel!.UserId);
                Response.Cookies.Append(
                    Options.CookieName,
                    newCookie,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        MaxAge = Options.ExpireTimeSpan,
                    }
                );
            }
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "验证用户权限出现异常");
            return AuthenticateResult.Fail("Authorize failed");
        }
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
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
        var result = new JsonResult(
            new UnifyResult()
            {
                success = false,
                code = 401,
                msg = "Authorized failed",
            }
        );

        var executor = Context.RequestServices.GetRequiredService<
            IActionResultExecutor<JsonResult>
        >();
        var routeData = Context.GetRouteData() ?? new RouteData();
        var actionContext = new ActionContext(Context, routeData, new ActionDescriptor());
        return executor.ExecuteAsync(actionContext, result);
    }
}
