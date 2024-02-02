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

namespace LightApi.Core.Authorization.Api;

public class CustomApiAuthHandler : AuthenticationHandler<CustomApiAuthSchemeOptions>
{
    public CustomApiAuthHandler(IOptionsMonitor<CustomApiAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers[HeaderNames.Authorization].ToString();

        // 如果需要 则还需要校验 Api开头
        if (string.IsNullOrWhiteSpace(header))
        {
            return Task.FromResult(AuthenticateResult.Fail("Api Header Not Found."));
        }

        var tokenArr=header.Split(" ");
        // 如果需要 则还需要校验 Api开头
        if (tokenArr is not ["Api", _])
        {
            return Task.FromResult(AuthenticateResult.Fail("Api Header Not Found."));
        }


        try
        {
            var token = header.Split(" ").Last();
            
            var validateResult=Validate(Context,token);
            
            // if(validateResult.code==1)
            //     return Task.FromResult(AuthenticateResult.Fail(BusinessErrorCode.Code401.GetDescription()));
            // if(validateResult.code==2)
            //     return Task.FromResult(AuthenticateResult.Fail(BusinessErrorCode.Code402.GetDescription()));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, validateResult.context?.UserName??""),
                new Claim(ClaimTypes.Role, validateResult.context?.Roles??""),
            };
            
            var claimsIdentity = new ClaimsIdentity(claims,
                Scheme.Name);

            // generate AuthenticationTicket from the Identity
            // and current authentication scheme
            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(claimsIdentity), Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception)
        {
            return Task.FromResult(AuthenticateResult.Fail(BusinessErrorCode.Code402.GetDescription()));
        }
    }

    private (int code,UserContext? context) Validate(HttpContext context, string token)
    {
        var tokenGenerator = context.RequestServices.GetRequiredService<ApiTokenGenerator>();
        try
        {
            var result = tokenGenerator.ResolveApiToken(token);
           
            if (result.opCode == 0)
            {
                var userContext = context.RequestServices.GetService<UserContext>();
                result.userContext.Adapt(userContext);
                return (0,userContext);
            }

            if (result.opCode == 1)
            {
                return (1,default);
            }
            
            if (result.opCode == 2)
            {
                return (2,default);
            }
            
            return (1,default);
        }
        catch (Exception e)
        {
            return (1,default);
        }
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
        //     msg = "非法访问"
        // });
        //
        // var executor = Context.RequestServices.GetRequiredService<IActionResultExecutor<JsonResult>>();
        // var routeData = Context.GetRouteData() ?? new RouteData();
        // var actionContext = new ActionContext(Context, routeData, new ActionDescriptor());
        // return executor.ExecuteAsync(actionContext, result);
        return Task.CompletedTask;
    }
}