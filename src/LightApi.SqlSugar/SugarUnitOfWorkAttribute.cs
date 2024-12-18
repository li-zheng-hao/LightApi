using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace LightApi.SqlSugar;

public class SugarUnitOfWorkAttribute : ActionFilterAttribute
{
    private ISqlSugarClient _client;
    private ILogger<SugarUnitOfWorkAttribute> _logger;

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        _client = context.HttpContext.RequestServices.GetRequiredService<ISqlSugarClient>();
        _logger = context.HttpContext.RequestServices.GetRequiredService<
            ILogger<SugarUnitOfWorkAttribute>
        >();

        await _client!.Ado.BeginTranAsync();

        var result = await next();

        if (result.Exception == null)
        {
            await _client!.Ado.CommitTranAsync();
        }
        else
        {
            _logger!.LogWarning(result.Exception, "工作单元异常回滚");
            await _client!.Ado.RollbackTranAsync();
        }
    }
}
