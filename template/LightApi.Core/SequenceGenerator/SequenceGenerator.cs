using LightApi.Core.Autofac;
using LightApi.Core.Extension;
using StackExchange.Redis;

namespace LightApi.Core.SequenceGenerator;

public class SequenceGenerator:ISingletonDependency
{
    private readonly ConnectionMultiplexer _redisClient;

    public SequenceGenerator(ConnectionMultiplexer redisClient)
    {
        _redisClient = redisClient;
    }
    /// <summary>
    /// 生成流水号 格式: 前缀+yyMMdd+6位自增数字
    /// </summary>
    /// <param name="key">序列号类型</param>
    /// <param name="SequenceResetType">流水号自增部分重置时间 默认按日</param>
    /// <param name="timeFormat">日期部分格式</param>
    /// <param name="length">流水号自增部分长度</param>
    /// <returns></returns>
    public async Task<string> GenerateSequenceAsync(string key,string prefix="",SequenceResetType resetType=SequenceResetType.Day
        ,string timeFormat="yyMMdd",int length = 6)
    {
        DateTime expireTime=DateTime.MinValue;
        if(resetType==SequenceResetType.Day)
            expireTime=DateTime.Today.GetNextDay();
        if(resetType==SequenceResetType.Month)
            expireTime=DateTime.Today.GetNextMonth();
        var num = await IncrementAsync(key,expireTime);
        var seqNum=num.ToString().PadLeft(length, '0');
        var date = DateTime.Now.ToString(timeFormat);
        return $"{prefix}{date}{seqNum}";
    }
    
    /// <summary>
    /// 指定key自增 原子操作
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireTime">指定过期时间，null则永不过期</param>
    /// <returns></returns>
    private async Task<long> IncrementAsync(string key, DateTime? expireTime=null)
    {
        var num=await _redisClient.GetDatabase().StringIncrementAsync(key);
        if(expireTime!=null)
            await _redisClient.GetDatabase().KeyExpireAsync(key, expireTime);
        return num;
    }
  
    
}