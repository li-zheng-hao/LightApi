using LightApi.Infra.Extension;
using LightApi.Infra.Options;
using LightApi.Infra.Unify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LightApi.Infra.AOP.Filters;

/// <summary>
/// 请求验证错误处理过滤器
/// </summary>
public class ModelValidatorFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext actionContext)
    {
        var options = actionContext.HttpContext.RequestServices.GetService<IOptions<InfrastructureOptions>>();
        var modelState = actionContext.ModelState;

        if (!modelState.IsValid)
        {
            List<string> errors = new List<string>();

            foreach (var key in modelState.Keys)
            {
                var state = modelState[key];

                if (state == null || !state.Errors.Any()) continue;
                
                var errorMessage = state.Errors?.FirstOrDefault()?.ErrorMessage;
                
                if (errorMessage != null)
                    errors.Add(errorMessage);
            }

            string? errorMsg =  errors.FirstOrDefault(it => it.HasChineseChar());

            if (errorMsg.IsNullOrWhiteSpace()) errorMsg = options!.Value.DefaultModelValidateErrorMessage;
            
            var unifyResultProvider = actionContext.HttpContext.RequestServices.GetService<IUnifyResultProvider>();
            var baseResult = unifyResultProvider!.Failure(errors, options!.Value.DefaultModelValidateErrorBusinessCode,
                options.Value.UseFirstModelValidateErrorMessage?errorMsg:options.Value.DefaultModelValidateErrorMessage,
                options.Value.DefaultModelValidateErrorHttpStatusCode
            );
            
            // 设置返回值和状态码
            actionContext.Result = new JsonResult(baseResult);
            actionContext.HttpContext.Response.StatusCode = (int)options.Value.DefaultModelValidateErrorHttpStatusCode;
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}