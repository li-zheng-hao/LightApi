using CliWrap;
using LightApi.Infra.Extension;
using Masuit.Tools;
using Serilog;

namespace LightApi.Infra.Helper;

using Newtonsoft.Json;

/// <summary>
/// 进程调用工具类
/// </summary>
public static class ProcessInvokeHelper
{
    /// <summary>
    /// 基本目录
    /// </summary>
    public static string BasePath { get; set; } =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProcessMessages");

    /// <summary>
    /// 把对象写入文件
    /// </summary>
    /// <param name="data"></param>
    /// <param name="path">文件完整路径 不填写自动生成</param>
    /// <param name="serializerSettings">Json序列化配置</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>文件路径</returns>
    public static string? Write<T>(
        T? data,
        string? path = null,
        JsonSerializerSettings? serializerSettings = null
    )
    {
        if (data == null)
            return default;

        if (string.IsNullOrWhiteSpace(path))
        {
            var fileName = $"{Guid.NewGuid().ToString()}.json";

            path = Path.Combine(BasePath, fileName);
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        if (data is string dataStr)
        {
            File.WriteAllText(path, dataStr);
        }
        else
        {
            dataStr = JsonConvert.SerializeObject(data, serializerSettings);
            File.WriteAllText(path, dataStr);
        }

        return path;
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="deleteAfterRead">读取完成后自动删除</param>
    /// <param name="serializerSettings">Json序列化配置</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? Read<T>(
        string? path,
        bool deleteAfterRead = true,
        JsonSerializerSettings? serializerSettings = null
    )
    {
        try
        {
            if (path == null || !File.Exists(path))
                return default;
            var dataStr = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(dataStr, serializerSettings);
        }
        finally
        {
            if (File.Exists(path) && deleteAfterRead)
                File.Delete(path);
        }
    }

    /// <summary>
    /// 进程调用
    /// </summary>
    /// <param name="configure"></param>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<(TResult? Response, CommandResult CommandResult)> ExecuteAsync<
        TParam,
        TResult
    >(Action<InvokeOptions<TParam>> configure)
    {
        var param = new InvokeOptions<TParam>();

        configure(param);

        if (param.WorkingDir.IsNullOrEmpty())
            param.WorkingDir = Path.GetDirectoryName(param.ExecutePath);

        param.LogOutput ??= Log.Debug;

        param.LogError ??= Log.Error;

        param.CancellationToken ??= new CancellationTokenSource(
            TimeSpan.FromSeconds(param.Timeout ?? 60)
        ).Token;

        var paramPath = param.JsonParamFileDir.IsNotNullOrWhiteSpace()
            ? Write(
                param.JsonParam,
                $"{Path.Combine(param.JsonParamFileDir!, $"{Guid.NewGuid()}.json")}",
                param.JsonSerializerSettings
            )
            : Write(param.JsonParam, null, param.JsonSerializerSettings);

        var args = param.Args.ToList();

        if (paramPath != null)
        {
            if (param.JsonParamAppendEnd ?? false)
                args.Add(paramPath);
            else
                args.Insert(0, paramPath);
        }

        var result = await Cli.Wrap(param.ExecutePath)
            .WithArguments(args)
            .WithWorkingDirectory(param.WorkingDir!)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(param.LogOutput))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(param.LogError))
            .WithValidation(
                param.IgnoreError
                    ? CommandResultValidation.None
                    : CommandResultValidation.ZeroExitCode
            )
            .ExecuteAsync(param.CancellationToken.Value);

        return (Read<TResult>(paramPath, true, param.JsonSerializerSettings), result);
    }

    /// <summary>
    /// 进程调用
    /// </summary>
    /// <param name="configure"></param>
    /// <typeparam name="TParam"></typeparam>
    /// <returns></returns>
    public static async Task<CommandResult> ExecuteAsync<TParam>(
        Action<InvokeOptions<TParam>> configure
    )
    {
        var param = new InvokeOptions<TParam>();

        configure(param);

        if (param.WorkingDir.IsNullOrEmpty())
            param.WorkingDir = Path.GetDirectoryName(param.ExecutePath);

        param.LogOutput ??= Log.Debug;

        param.LogError ??= Log.Error;

        param.CancellationToken ??= new CancellationTokenSource(
            TimeSpan.FromSeconds(param.Timeout ?? 60)
        ).Token;

        var paramPath = param.JsonParamFileDir.IsNotNullOrWhiteSpace()
            ? Write(
                param.JsonParam,
                $"{Path.Combine(param.JsonParamFileDir!, $"{Guid.NewGuid()}.json")}",
                param.JsonSerializerSettings
            )
            : Write(param.JsonParam, null, param.JsonSerializerSettings);

        var args = param.Args.ToList();

        if (paramPath != null)
        {
            if (param.JsonParamAppendEnd ?? false)
                args.Add(paramPath);
            else
                args.Insert(0, paramPath);
        }

        var result = await Cli.Wrap(param.ExecutePath)
            .WithArguments(args)
            .WithWorkingDirectory(param.WorkingDir!)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(param.LogOutput))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(param.LogError))
            .WithValidation(
                param.IgnoreError
                    ? CommandResultValidation.None
                    : CommandResultValidation.ZeroExitCode
            )
            .ExecuteAsync(param.CancellationToken.Value);

        if (File.Exists(paramPath))
            File.Delete(paramPath);

        return (result);
    }
}

public class InvokeOptions<T>
{
    /// <summary>
    /// 附加参数
    /// </summary>
    public string[] Args { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 超时  默认60秒
    /// 也可以直接配置Timeout来设置超时时间（秒）<seealso cref="Timeout" />
    /// </summary>
    public CancellationToken? CancellationToken { get; set; }

    /// <summary>
    /// 序列化为json数据的参数 会自动写入json文件 不填则跳过
    /// </summary>
    public T? JsonParam { get; set; }

    /// <summary>
    /// 执行路径
    /// </summary>
    public string ExecutePath { get; set; }

    /// <summary>
    /// 默认输出到Log.Debug
    /// </summary>
    public Action<string>? LogOutput { get; set; }

    /// <summary>
    /// 默认输出到Log.Error
    /// </summary>
    public Action<string>? LogError { get; set; }

    /// <summary>
    /// 工作目录 为空时会自动从<see cref="ExecutePath"/>计算
    /// </summary>
    public string? WorkingDir { get; set; }

    /// <summary>
    /// 当进程返回非0代码时是否忽略错误 默认true
    /// </summary>
    public bool IgnoreError { get; set; } = true;

    /// <summary>
    /// 超时时间 默认60秒
    /// </summary>
    public uint? Timeout { get; set; }

    /// <summary>
    /// Json参数文件是否放在结尾
    /// </summary>
    public bool? JsonParamAppendEnd { get; set; } = false;

    /// <summary>
    /// json参数文件所在目录，为空的话使用BasePath
    /// </summary>
    public string? JsonParamFileDir { get; set; }

    /// <summary>
    /// Json序列化配置，null则使用默认配置
    /// </summary>
    public JsonSerializerSettings? JsonSerializerSettings { get; set; }
}
