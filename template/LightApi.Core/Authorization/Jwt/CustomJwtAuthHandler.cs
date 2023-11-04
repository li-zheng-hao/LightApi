using System.Security.Claims;
using System.Text.Encodings.Web;
using Mapster;
using Masuit.Tools.Systems;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace LightApi.Core.Authorization.Jwt;

public class CustomJwtAuthHandler : AuthenticationHandler<CustomJwtAuthSchemeOptions>
{
    public CustomJwtAuthHandler(IOptionsMonitor<CustomJwtAuthSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers[HeaderNames.Authorization].ToString();

        var tokenArr = header.Split(" ");
        
        if (tokenArr is not [CustomAuthorizationSchemes.JwtSchemeName, _])
        {
            return Task.FromResult(AuthenticateResult.Fail(BusinessErrorCode.Code401.GetDescription()));
        }

        try
        {
            var token = tokenArr[1];

            var result = Validate(Context, token);
            
            if (result.code == 1)
            {
                return Task.FromResult(AuthenticateResult.Fail(BusinessErrorCode.Code402.GetDescription()));
            }
            
            if (result.code == 2)
                return Task.FromResult(AuthenticateResult.Fail(BusinessErrorCode.Code401.GetDescription()));
            
            var tokenModel = result.context;
            
            var userContext = Context.RequestServices.GetService<UserContext>();
            
            tokenModel.Adapt(userContext);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result.context.Name),
                new Claim(ClaimTypes.Role, result.context.Roles),
            };


            var claimsIdentity = new ClaimsIdentity(claims,
                nameof(CustomJwtAuthHandler));

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(claimsIdentity), Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception)
        {
            return Task.FromResult(AuthenticateResult.Fail(BusinessErrorCode.Code401.GetDescription()));
        }
    }

    private (int code, UserContext context) Validate(HttpContext context, string token)
    {
        var tokenGenerator = context.RequestServices.GetRequiredService<JwtTokenGenerator>();
        
        // TODO
     
        var userContext= new UserContext();
       
        return (0, userContext);
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
        //     code = 401,
        //     msg = BusinessErrorCode.Code401.GetDescription()
        // });

        // var executor = Context.RequestServices.GetRequiredService<IActionResultExecutor<JsonResult>>();
        // var routeData = Context.GetRouteData() ?? new RouteData();
        // var actionContext = new ActionContext(Context, routeData, new ActionDescriptor());
        // return executor.ExecuteAsync(actionContext, result);
        return Task.CompletedTask;
    }
}