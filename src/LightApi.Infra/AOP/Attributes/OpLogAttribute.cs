using System.Diagnostics;
using LightApi.Infra.Extension;
using LightApi.Infra.Json;
using LightApi.Infra.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rougamo;
using Rougamo.Context;
using Serilog;
using Serilog.Events;

namespace LightApi.Infra.AOP.Attributes;

/// <summary>
/// 操作AOP日志特性
/// </summary>
public class OpLogAttribute : ExMoAttribute
{
    public string OpName { get; set; }

    public LogEventLevel Level { get; set; } = LogEventLevel.Debug;

    public uint? LogLength { get; set; }

    private Stopwatch _sw;

    private DateTime _beginTime;

    private readonly Type[] _ignoreTypes =
    {
        typeof(CancellationToken)
    };

    public OpLogAttribute(string opName = "")
    {
        if (opName.IsNotNullOrWhiteSpace())
        {
            OpName = opName;
        }

        LogLength ??= App.GetService<IOptions<InfrastructureOptions>>()?.Value.MaxLogLength;
    }

    protected override void ExOnEntry(MethodContext context)
    {
        _sw = new Stopwatch();
        _sw.Start();
        _beginTime = DateTime.Now;
    }

    protected override void ExOnExit(MethodContext context)
    {
        var parameterStr = JsonConvert.SerializeObject(context.Arguments
            .Where(it => !_ignoreTypes.Contains(it.GetType())),new JsonSerializerSettings(){ ContractResolver = new DynamicContractResolver() });

        parameterStr = parameterStr.SafeSubString(LogLength!.Value);


        if (context.Exception is not null)
        {
            Log.Error(context.Exception, "操作日志(出现异常) 描述: {0} 请求路由: {1}  请求时间: {2} 结束时间: {3} 请求参数: {4} 耗时: {6}毫秒",
                OpName, context.Method.Name, _beginTime, DateTime.Now, parameterStr, _sw.ElapsedMilliseconds);
        }
        else
        {
            var resultStr = 
                JsonConvert.SerializeObject(context.ExReturnValue
                    ,new JsonSerializerSettings(){ ContractResolver = new DynamicContractResolver() });
            resultStr = resultStr.SafeSubString(LogLength!.Value);

            Log.Write(Level, "操作日志 描述: {0} 请求路由: {1}  请求时间: {2} 结束时间: {3} 请求参数: {4} 返回结果: {5} 耗时: {6}毫秒",
                OpName, context.Method.Name, _beginTime, DateTime.Now, parameterStr, resultStr, _sw.ElapsedMilliseconds);
        }
    }
}