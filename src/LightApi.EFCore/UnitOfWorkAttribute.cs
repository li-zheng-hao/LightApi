using LightApi.EFCore.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.EFCore;

public class UnitOfWorkAttribute : ActionFilterAttribute
{
    public Guid Guid { get; set; }= System.Guid.NewGuid();
    /// <summary>
    /// 是否共享给CAP
    /// </summary>
    public bool SharedToCap { get; set; } = false;

    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

    /// <summary>
    /// 数据库上下文 如果不设置的话则获取默认主库的配置
    /// </summary>
    public Type? DbContextType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IUnitOfWork uow;
        
        if (DbContextType == null)
        {
            uow = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        }
        else
        {
            var uowType=Db.GetDbContextUnitOfWorkMap(DbContextType);
            uow = (context.HttpContext.RequestServices.GetRequiredService(uowType) as IUnitOfWork)!;
        }
        
        uow.BeginTransaction(IsolationLevel, SharedToCap);
        
        var result = await next();

        if (result.Exception is null)
        {
            await uow.CommitAsync();
        }
        else
        {
            await uow.RollbackAsync();
        }
    }
}