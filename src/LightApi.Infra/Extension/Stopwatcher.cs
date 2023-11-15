using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LightApi.Infra.Extension;

/// <summary>
/// 记录耗时工具类
/// </summary>
public class Stopwatcher
{
    private readonly ILogger? _logger;

    private readonly string _prefix;

    private bool _restart = true;
    private Stopwatch _watcher { get; set; }
    
    public Stopwatcher(string prefix="",ILogger? logger=null,bool restart=true)
    {
        _restart = true;
        _logger = logger;
        _prefix = string.IsNullOrWhiteSpace(prefix)?"":$"{prefix}:";
        _watcher = new Stopwatch();
        _watcher.Restart();
    }

    /// <summary>
    /// 记录耗时
    /// </summary>
    /// <param name="section">当前记录区间的描述</param>
    /// <param name="restart">是否在记录此次耗时后重新开始新计时，默认false</param>
    public void Log(string section,bool restart)
    {
        _restart=restart;
        
        Log(section);
    }
    /// <summary>
    /// 记录耗时
    /// </summary>
    /// <param name="section">当前记录区间的描述</param>
    /// <param name="restart">是否在记录此次耗时后重新开始新计时，默认false</param>
    public void Log(string section)
    {
        if (_logger == null)
            Console.WriteLine($"{_prefix}{section}耗时 : {_watcher.ElapsedMilliseconds}ms");
        else
            _logger.LogDebug($"{_prefix}{section}耗时 : {_watcher.ElapsedMilliseconds}ms");

        if (_restart)
            _watcher.Restart();
    }
}