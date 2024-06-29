using System.Reflection;
using LightApi.Infra.Extension;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LightApi.Infra.Swagger;

/// <summary>
/// 过时数据过滤器
/// </summary>
public class ObsoleteOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Deprecated)
        {
            var obsoleteMsg=context.MethodInfo.DeclaringType?.GetCustomAttribute<ObsoleteAttribute>()?.Message;
            if (obsoleteMsg.IsNullOrWhiteSpace())
            {
                obsoleteMsg = context.MethodInfo.GetCustomAttribute<ObsoleteAttribute>()?.Message;
            }
            if (obsoleteMsg.IsNotNullOrWhiteSpace())
                operation.Summary = $"{operation.Summary}   [{obsoleteMsg}]";
            
            operation.Summary =
                    $"{operation.Summary}   [此接口已废弃，可能在后续版本删除，请及时更新！！]";
        }

        // operation.Description = operation.Deprecated
        //     ? context.MethodInfo.GetCustomAttribute<ObsoleteAttribute>()?.Message
        //     : operation.Description;
    }
}