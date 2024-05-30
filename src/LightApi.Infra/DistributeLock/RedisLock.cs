using Serilog;
using StackExchange.Redis;

namespace LightApi.Infra.DistributeLock;

public class RedisLock:IAsyncDisposable,IDisposable
{

    public RedisLock(string[] lockKeys, string lockValue, uint timeout, ConnectionMultiplexer client)
    {
        _lockKeys = lockKeys;
        _lockValue = lockValue;
        _timeout = timeout;
        _client = client;
        _timer = new Timer(Renew, null, TimeSpan.FromSeconds(timeout*0.6), TimeSpan.FromSeconds(timeout*0.6));
    }

    private readonly Timer _timer;

    private uint _timeout { get; set; }
    /// <summary>
    /// 锁key
    /// </summary>
    private string[] _lockKeys { get; set; }
    
    /// <summary>
    /// 锁值
    /// </summary>
    private string _lockValue { get; set; }

    private ConnectionMultiplexer _client { get; set; }

    private bool _disposed { get; set; }
    private void Release()
    {
        // Lua脚本
        var keys=_lockKeys.Select(it => new RedisKey(it)).ToArray();
        // 执行Lua脚本
        var result =_client.GetDatabase().ScriptEvaluate(unlockScript, keys, new RedisValue[] { 1 });

        if ((int)result != 1)
        {
            Log.Error($"分布式锁{string.Join(",",_lockKeys)}解锁失败");
        }

        _timer.Dispose();

        _disposed = true;
    }
    
    private async Task ReleaseAsync()
    {
        // Lua脚本
        var keys=_lockKeys.Select(it => new RedisKey(it)).ToArray();
        // 执行Lua脚本
        var result =await _client.GetDatabase().ScriptEvaluateAsync(unlockScript, keys, new RedisValue[] { 1 });

        if ((int)result != 1)
        {
            Log.Error($"分布式锁{string.Join(",",_lockKeys)}解锁失败");
        }

        await _timer.DisposeAsync();

        _disposed = true;
    }
    
    public async ValueTask DisposeAsync()
    {
        await ReleaseAsync();
    }

    private void Renew(object? state)
    {
        // Lua脚本
        var keys=_lockKeys.Select(it => new RedisKey(it)).ToArray();
        
        // 将过期时间和锁值作为参数传递给Lua脚本
        var parameters = new RedisValue[] { _timeout, _lockValue };
        
        // 执行Lua脚本
        var result =_client.GetDatabase().ScriptEvaluate(renewScript, keys, parameters);

        if ((int)result != 1)
        {
            Log.Error($"分布式锁{string.Join(",",_lockKeys)}刷新失败");
        }
    }


    public void Dispose()
    {
        if (!_disposed)
        {
            Release();
        }
    }
    
    
    /// <summary>
    /// 解锁lua脚本
    /// </summary>
    private const string unlockScript = @"
        local keys = KEYS
        local lock_value = ARGV[1]

        for i, key in ipairs(keys) do
            redis.call('DEL', key)
        end

        return 1
    ";
    
    /// <summary>
    /// 解锁lua脚本
    /// </summary>
    private const string renewScript = @"
        local keys = KEYS
        local expire_time = ARGV[1]
        local lock_value = ARGV[2]

        for i, key in ipairs(keys) do
            redis.call('SET', key, lock_value, 'EX', expire_time)
        end

        return 1
    ";

}