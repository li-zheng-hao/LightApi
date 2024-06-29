using System.Diagnostics;
using LightApi.Infra.InfraException;
using StackExchange.Redis;

namespace LightApi.Infra.DistributeLock;

/// <summary>
/// redis分布式锁提供者
/// </summary>
public class RedisLockProvider
{
    private readonly ConnectionMultiplexer _redisClient;
    /// <summary>
    /// 未获取到锁时的刷新间隔最小值
    /// </summary>
    public int MinIntervalMilliSeconds => 10;
    /// <summary>
    /// 未获取到锁时的刷新间隔最大值
    /// </summary>
    public int MaxIntervalMilliSeconds => 500;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="redisClient"></param>
    public RedisLockProvider(ConnectionMultiplexer redisClient)
    {
        _redisClient = redisClient;
    }
    
    
    /// <summary>
    /// 尝试获取锁
    /// </summary>
    /// <param name="lockKey"></param>
    /// <param name="timeoutSeconds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public RedisLock? TryLock(string lockKey, uint timeoutSeconds=10)
    {
        if (timeoutSeconds < 2)
            throw new ArgumentException($"{nameof(timeoutSeconds)}不能小于2秒");
        var db = _redisClient!.GetDatabase();

        string lockValue = Guid.NewGuid().ToString();
        // 将过期时间和锁值作为参数传递给Lua脚本
        var parameters = new RedisValue[] { timeoutSeconds, lockValue };

        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        var keys=new []{lockKey}.Select(it => new RedisKey(it)).ToArray();
        
        while (true)
        {
            if (stopwatch.ElapsedMilliseconds > timeoutSeconds * 1000)
                return null;
            
            // 执行Lua脚本
            var result = db.ScriptEvaluate(lockStript, keys, parameters);

            if ((int)result == 1)
            {
                return new RedisLock(new []{lockKey}, lockValue, timeoutSeconds, _redisClient);
            }

            Task.Delay(Random.Shared.Next(MinIntervalMilliSeconds,MaxIntervalMilliSeconds)).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// 尝试获取锁
    /// </summary>
    /// <param name="lockKey"></param>
    /// <param name="timeoutSeconds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<RedisLock?> TryLockAsync(string lockKey, uint timeoutSeconds=10)
    {
        if (timeoutSeconds < 2)
            throw new ArgumentException($"{nameof(timeoutSeconds)}不能小于2秒");
        var db = _redisClient!.GetDatabase();

        string lockValue = Guid.NewGuid().ToString();
        // 将过期时间和锁值作为参数传递给Lua脚本
        var parameters = new RedisValue[] { timeoutSeconds, lockValue };

        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        var keys=new []{lockKey}.Select(it => new RedisKey(it)).ToArray();
        
        while (true)
        {
            if (stopwatch.ElapsedMilliseconds > timeoutSeconds * 1000)
                return null;
            
            // 执行Lua脚本
            var result = await db.ScriptEvaluateAsync(lockStript, keys, parameters);

            if ((int)result == 1)
            {
                return new RedisLock(new []{lockKey}, lockValue, timeoutSeconds, _redisClient);
            }

            await Task.Delay(Random.Shared.Next(MinIntervalMilliSeconds,MaxIntervalMilliSeconds));
        }
    }

    /// <summary>
    /// 尝试获取锁
    /// </summary>
    /// <param name="lockKeys"></param>
    /// <param name="timeoutSeconds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<RedisLock?> TryLockAsync(string[] lockKeys, uint timeoutSeconds=10)
    {
        if (timeoutSeconds < 2)
            throw new ArgumentException($"{nameof(timeoutSeconds)}不能小于2秒");
        var db = _redisClient!.GetDatabase();

        string lockValue = Guid.NewGuid().ToString();
        // 将过期时间和锁值作为参数传递给Lua脚本
        var parameters = new RedisValue[] { timeoutSeconds, lockValue };

        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        var keys=lockKeys.Select(it => new RedisKey(it)).ToArray();
        
        while (true)
        {
            if (stopwatch.ElapsedMilliseconds > timeoutSeconds * 1000)
                return null;
            
            // 执行Lua脚本
            var result = await db.ScriptEvaluateAsync(lockStript, keys, parameters);

            if ((int)result == 1)
            {
                return new RedisLock(lockKeys, lockValue, timeoutSeconds, _redisClient);
            }

            await Task.Delay(Random.Shared.Next(MinIntervalMilliSeconds,MaxIntervalMilliSeconds));
        }
    }
    
    
    /// <summary>
    /// 尝试获取锁
    /// </summary>
    /// <param name="lockKeys"></param>
    /// <param name="timeoutSeconds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public RedisLock? TryLock(string[] lockKeys, uint timeoutSeconds=10)
    {
        if (timeoutSeconds < 2)
            throw new ArgumentException($"{nameof(timeoutSeconds)}不能小于2秒");
        var db = _redisClient!.GetDatabase();

        string lockValue = Guid.NewGuid().ToString();
        // 将过期时间和锁值作为参数传递给Lua脚本
        var parameters = new RedisValue[] { timeoutSeconds, lockValue };

        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        var keys=lockKeys.Select(it => new RedisKey(it)).ToArray();
        
        while (true)
        {
            if (stopwatch.ElapsedMilliseconds > timeoutSeconds * 1000)
                return null;
            
            // 执行Lua脚本
            var result = db.ScriptEvaluate(lockStript, keys, parameters);

            if ((int)result == 1)
            {
                return new RedisLock(lockKeys, lockValue, timeoutSeconds, _redisClient);
            }

            Task.Delay(Random.Shared.Next(MinIntervalMilliSeconds,MaxIntervalMilliSeconds)).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// 加锁lua脚本
    /// </summary>
    private const string lockStript = @"
        local keys = KEYS
        local expire_time = ARGV[1]
        local lock_value = ARGV[2]

        -- 遍历所有键，检查是否存在
        for i, key in ipairs(keys) do
            if redis.call('EXISTS', key) == 1 then
                return 0
            end
        end

        -- 所有键都不存在，进行加锁
        for i, key in ipairs(keys) do
            redis.call('SET', key, lock_value, 'EX', expire_time)
        end

        return 1
    ";
 
}