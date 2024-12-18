using Rougamo;
using Rougamo.Context;
using Rougamo.Metadatas;
using Serilog.Context;

namespace LightApi.Infra.AOP.Attributes;

/// <summary>
/// 给日志上下文新增一个OperationId字段，方便日志查询过滤
/// </summary>
[Lifetime(Lifetime.Transient)]
public class AddOperationIdAttribute : MoAttribute
{
    /// <summary>
    ///
    /// </summary>
    public string OperationId { get; set; }
    private IDisposable? _context { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="operationId"></param>
    public AddOperationIdAttribute(string? operationId = null)
    {
        OperationId = operationId ?? Guid.NewGuid().ToString();
    }

    public override void OnEntry(MethodContext context)
    {
        Console.WriteLine("Entry");
        _context = LogContext.PushProperty(nameof(OperationId), OperationId);
    }

    public override void OnExit(MethodContext context)
    {
        Console.WriteLine("Exit");
        _context?.Dispose();
    }
}
