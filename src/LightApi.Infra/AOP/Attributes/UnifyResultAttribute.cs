using LightApi.Infra.Unify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FB.Infrastructure.AOP;

/// <summary>
/// 需要忽略包装的函数上加这个特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NonUnifyAttribute : Attribute
{
}

/// <summary>
/// 返回值统一包装
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UnifyResultAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
        var attributes = context.ActionDescriptor.EndpointMetadata.OfType<NonUnifyAttribute>();
        var option = context.HttpContext.RequestServices.GetService<IOptions<InfrastructureOptions>>();
        var unifyResultProvider = context.HttpContext.RequestServices.GetService<IUnifyResultProvider>();

        if (attributes.Any())
            return;

        if (context.Result is ObjectResult objRes)
        {
            if (context.Exception is not null)
                return;

            if (objRes.Value is IUnifyResult)
                return;

            context.Result = new ObjectResult(unifyResultProvider.Success(objRes.Value,
                option.Value.DefaultSuccessCode, option.Value.DefaultSuccessMessage));
        }
        else if (context.Result is NoContentResult or StatusCodeResult)
        {
            context.Result = new ObjectResult(unifyResultProvider.Success(null, option.Value.DefaultSuccessCode,
                option.Value.DefaultSuccessMessage
            ));
        }
    }
}