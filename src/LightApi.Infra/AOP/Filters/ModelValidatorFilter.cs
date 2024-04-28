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
                    errors.Add(GenerateFriendlyMessage(errorMessage));
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
    
    /// <summary>
    /// 硬编码处理NewtonsoftJson序列化错误信息，如果序列化框架有修改，需要调整（待优化）
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    private string GenerateFriendlyMessage(string error)
    {
        try
        {
            if(error.StartsWith("Could not convert string to integer"))
            {
                var inputValue =error.Substring("Could not convert string to integer ".Length).Split('.').First();
                return $"输入值{inputValue}无效，请输入有效范围内的整数";
            }
            
            if(error.Contains("is too large or small"))
            {
                var inputValue =error.Substring("JSON integer ".Length).Split(' ').First();
                return $"输入值{inputValue}无效，请输入有效范围内的整数";
            }
        }
        catch (Exception e)
        {
            // ignore
            return error;
        }
      
        return error;
    } 
}