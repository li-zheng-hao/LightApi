using Rougamo;
using Rougamo.Context;
using Serilog.Context;

namespace LightApi.Infra.AOP.Attributes;

/// <summary>
/// 给日志上下文新增一个OperationId字段，方便日志查询过滤
/// </summary>
public class AddOperationIdAttribute : ExMoAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public string OperationId { get; set; }

    private IDisposable? _context { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="OperationId"></param>
    public AddOperationIdAttribute(string? operationId =null)
    {
      
        OperationId = operationId??Guid.NewGuid().ToString();
    }

    protected override void ExOnEntry(MethodContext context)
    {
        _context=LogContext.PushProperty(nameof(OperationId), OperationId);
    }

    protected override void ExOnExit(MethodContext context)
    {
        _context?.Dispose();
    }
}