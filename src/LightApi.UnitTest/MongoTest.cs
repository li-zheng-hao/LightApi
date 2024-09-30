using LightApi.Mongo;
using LightApi.Mongo.Entities;
using LightApi.Mongo.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Yitter.IdGenerator;

namespace LightApi.UnitTest;

public class MongoTest
{
    [Fact]
    public async Task TransactionRollbackTest()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddMongoSetup("test", Environment.GetEnvironmentVariable("APP_MONGO_CONNECTIONSTRING"));
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var unitOfWork = serviceProvider.GetService<IMongoUnitOfWork>();
        unitOfWork.StartTransaction(serviceProvider);
        var dbContext = serviceProvider.GetService<DBContext>();
        var model=new ModelTest() { Name = Guid.NewGuid().ToString() };
        await dbContext.SaveAsync(model);
        await unitOfWork.RollbackAsync();
        var modelExist = await dbContext.Queryable<ModelTest>().FirstOrDefaultAsync(it=>it.Name==model.Name);
        Assert.Null(modelExist);
    }
    [Fact]
    public async Task SnowflakeModel()
    {
        var options = new IdGeneratorOptions(1);
// options.WorkerIdBitLength = 10; // 默认值6，限定 WorkerId 最大值为2^6-1，即默认最多支持64个节点。
// options.SeqBitLength = 6; // 默认值6，限制每毫秒生成的ID个数。若生成速度超过5万个/秒，建议加大 SeqBitLength 到 10。
// options.BaseTime = Your_Base_Time; // 如果要兼容老系统的雪花算法，此处应设置为老系统的BaseTime。
// ...... 其它参数参考 IdGeneratorOptions 定义。
        options.BaseTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local);
// 保存参数（务必调用，否则参数设置不生效）：
        YitIdHelper.SetIdGenerator(options);
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddMongoSetup("test", Environment.GetEnvironmentVariable("APP_MONGO_CONNECTIONSTRING"));
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var model=new ModelSnowflakeTest()
        {
            Name = "test"
        };
        await model.SaveAsync();
        var modelExist=DB.Queryable<ModelSnowflakeTest>().FirstOrDefault();
        Assert.Equal(model.Id, modelExist!.Id);
    }
    internal class ModelTest:MongoEntity
    {
        public string Name { get; set; }
        
        public string Version { get; set; }
    }
    internal class ModelSnowflakeTest:MongoSnowflakeEntity
    {
        public string Name { get; set; }
    }
}