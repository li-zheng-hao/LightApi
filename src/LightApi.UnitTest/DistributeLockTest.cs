using LightApi.Infra.DistributeLock;
using StackExchange.Redis;

namespace LightApi.UnitTest;

public class DistributeLockTest
{
    [Fact]
    public async Task MultiLockTestAsync()
    {
        ConnectionMultiplexer redis =
            ConnectionMultiplexer.Connect("localhost:6379,password=chaojiyonghu");

        var redisLockProvider = new RedisLockProvider(redis);
        var lockKeys = new string[] { "key1", "key2" };

        var locker = await redisLockProvider.TryLockAsync(lockKeys);

        Assert.NotNull(locker);

        var key1 = redis.GetDatabase().StringGet("key1");
        var key2 = redis.GetDatabase().StringGet("key2");
        Assert.True(key1.HasValue);
        Assert.True(key2.HasValue);

        await locker.DisposeAsync();


        key1 = redis.GetDatabase().StringGet("key1");
        key2 = redis.GetDatabase().StringGet("key2");
        Assert.False(key1.HasValue);
        Assert.False(key2.HasValue);
    }

    [Fact]
    public async Task MultiLockTestWithRefreshAsync()
    {
        ConnectionMultiplexer redis =
            ConnectionMultiplexer.Connect("localhost:6379,password=chaojiyonghu");

        var redisLockProvider = new RedisLockProvider(redis);
        var lockKeys = new string[] { "key1", "key2" };

        var locker = await redisLockProvider.TryLockAsync(lockKeys, 5);

        Assert.NotNull(locker);

        await Task.Delay(10000);

        var key1 = redis.GetDatabase().StringGet("key1");
        var key2 = redis.GetDatabase().StringGet("key2");
        Assert.True(key1.HasValue);
        Assert.True(key2.HasValue);

        await locker.DisposeAsync();


        key1 = redis.GetDatabase().StringGet("key1");
        key2 = redis.GetDatabase().StringGet("key2");
        Assert.False(key1.HasValue);
        Assert.False(key2.HasValue);
    }


    [Fact]
    public void MultiLockTest()
    {
        ConnectionMultiplexer redis =
            ConnectionMultiplexer.Connect("localhost:6379,password=chaojiyonghu");

        var redisLockProvider = new RedisLockProvider(redis);
        var lockKeys = new string[] { "key1", "key2" };

        var locker = redisLockProvider.TryLock(lockKeys);

        Assert.NotNull(locker);

        var key1 = redis.GetDatabase().StringGet("key1");
        var key2 = redis.GetDatabase().StringGet("key2");
        Assert.True(key1.HasValue);
        Assert.True(key2.HasValue);

        locker.Dispose();

        key1 = redis.GetDatabase().StringGet("key1");
        key2 = redis.GetDatabase().StringGet("key2");
        Assert.False(key1.HasValue);
        Assert.False(key2.HasValue);
    }

    [Fact]
    public void SingleLockTest()
    {
        ConnectionMultiplexer redis =
            ConnectionMultiplexer.Connect("localhost:6379,password=chaojiyonghu");

        var redisLockProvider = new RedisLockProvider(redis);
        var key = "key1";
        var locker = redisLockProvider.TryLock(key);

        Assert.NotNull(locker);

        var key1 = redis.GetDatabase().StringGet("key1");
        Assert.True(key1.HasValue);

        locker.Dispose();

        key1 = redis.GetDatabase().StringGet("key1");
        Assert.False(key1.HasValue);
    }

    [Fact]
    public async Task SingleLockTestAsync()
    {
        ConnectionMultiplexer redis =
            ConnectionMultiplexer.Connect("localhost:6379,password=chaojiyonghu");

        var redisLockProvider = new RedisLockProvider(redis);
        var key = "key1";
        var locker = await redisLockProvider.TryLockAsync(key);

        Assert.NotNull(locker);

        var key1 = redis.GetDatabase().StringGet("key1");
        Assert.True(key1.HasValue);

        await locker.DisposeAsync();

        key1 = redis.GetDatabase().StringGet("key1");
        Assert.False(key1.HasValue);
    }

    [Fact]
    public void ParallelLockTestAsync()
    {
        ConnectionMultiplexer redis =
            ConnectionMultiplexer.Connect("localhost:6379,password=chaojiyonghu");

        var redisLockProvider = new RedisLockProvider(redis);

        var key = new[] { "key1", "key2" };

        int lockerCount = 0;

        var arr = Enumerable.Range(1, 5).ToList();

        Parallel.ForEach(arr, it =>
        {
            var locker = redisLockProvider.TryLock(key, 2);
            if (locker != null)
                Interlocked.Increment(ref lockerCount);
        });
        Assert.Equal(1, lockerCount);
    }
}