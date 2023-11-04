using LightApi.Infra.Unify;
using Masuit.Tools.Systems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.Core.Authorization;

public class PermissionCheckAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowPermissons;

    public PermissionCheckAttribute(params string[] allowPermissons)
    {
        _allowPermissons = allowPermissons;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(it => it is AllowAnonymousAttribute))
            return;
#if DEBUG
        if (context.HttpContext.Request.Headers["Referer"].ToString().Contains("swagger") &&
            string.IsNullOrWhiteSpace(context.HttpContext.Request.Headers.Authorization.ToString()))
            return;
#endif
        var userContext = context.HttpContext.RequestServices.GetService<UserContext>();

        // var data = GetAllRolePermission(context);
        //
        // var passed = HasPermission(userContext, data);
        //
        //
        // if (!passed)
        //     Return403(context);
    }

    // private bool HasPermission(UserContext userContext,
    //     List<Tuple<AuthUserRole, List<AuthUserPermission>>>? allPermissions)
    // {
    //     if (string.IsNullOrWhiteSpace(userContext.Roles))
    //         return false;
    //     foreach (var role in userContext.Roles.Split(","))
    //     {
    //         var rolePermission = allPermissions?.FirstOrDefault(it => it.Item1.Name == role);
    //         if (rolePermission == null)
    //             continue;
    //         if (rolePermission.Item2.Any(it =>
    //                 _allowPermissons.Contains(it.Name) || it.Name == PermissionKeys.SuperPermission))
    //             return true;
    //     }
    //
    //     return false;
    // }
    //
    // private void Return403(AuthorizationFilterContext context)
    // {
    //     context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
    //     context.Result = new JsonResult(new UnifyResult()
    //     {
    //         code = (int)BusinessErrorCode.Code403,
    //         msg = BusinessErrorCode.Code403.GetDescription()
    //     });
    // }
}