using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.EFCore;

public class UnitOfWorkAttribute : ActionFilterAttribute
{
    public bool SharedToCap { get; set; } = false;

    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

    private IUnitOfWork _uow;


    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _uow = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        _uow.BeginTransaction(IsolationLevel, SharedToCap);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is null)
        {
            _uow.Commit();
        }
        else
        {
            _uow.Rollback();
        }
    }

    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _uow = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        
        _uow.BeginTransaction(IsolationLevel, SharedToCap);
        
        var result = await next();

        if (result.Exception is null)
        {
            await _uow.CommitAsync();
        }
        else
        {
            await _uow.RollbackAsync();
        }
    }
}