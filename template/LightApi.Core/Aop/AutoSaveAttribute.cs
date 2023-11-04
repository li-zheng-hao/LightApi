using LightApi.EFCore.EFCore.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LightApi.Core.Aop;

/// <summary>
/// 自动保存 排除Get请求
/// </summary>
public class AutoSaveAttribute:ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        
        // get action attribute
        var getAttribute = context.ActionDescriptor.EndpointMetadata.OfType<HttpGetAttribute>().FirstOrDefault();
        
        var res=await next();

        if(getAttribute!=null) return ;
        
        if(res.Exception!=null) return ;
        
        var db=context.HttpContext.RequestServices.GetService(typeof(AppDbContext)) as AppDbContext;

        await db!.SaveChangesAsync();
    }
}