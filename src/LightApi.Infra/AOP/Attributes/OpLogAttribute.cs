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
            .Where(it => it!=null&&!_ignoreTypes.Contains(it.GetType())),new JsonSerializerSettings(){ ContractResolver = new DynamicContractResolver() });

        parameterStr = parameterStr.SafeSubString(LogLength!.Value);


        if (context.Exception is not null)
        {
            Log.Error(context.Exception, "\r\n------------------------------------\r\n操作日志(出现异常) \r\n描述: {0} \r\n请求方法: {1}  \r\n请求时间: {2} \r\n结束时间: {3} \r\n请求参数: {4} \r\n耗时: {6}毫秒\r\n------------------------------------",
                OpName, context.Method.Name, _beginTime, DateTime.Now, parameterStr, _sw.ElapsedMilliseconds);
        }
        else
        {
            var resultStr = 
                JsonConvert.SerializeObject(context.ExReturnValue
                    ,new JsonSerializerSettings(){ ContractResolver = new DynamicContractResolver() });
            resultStr = resultStr.SafeSubString(LogLength!.Value);

            Log.Write(Level, "\r\n------------------------------------\r\n操作日志 \r\n描述: {0} \r\n请求方法: {1} \r\n请求时间: {2} \r\n结束时间: {3} \r\n请求参数: {4} \r\n返回结果: {5} \r\n耗时: {6}毫秒\r\n------------------------------------",
                OpName, context.Method.Name, _beginTime, DateTime.Now, parameterStr, resultStr, _sw.ElapsedMilliseconds);
        }
    }
}