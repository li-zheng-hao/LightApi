using LightApi.Mongo.Entities;
using LightApi.Mongo.Repository;
using LightApi.Mongo.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Entities;

namespace LightApi.UnitTest;

public class MongoTest
{
    [Fact]
    public async Task PageQuery()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(typeof(MongoRepository<>));
        serviceCollection.AddScoped(typeof(IMongoUnitOfWork),typeof(MongoUnitOfWork));
        DB.InitAsync("TestDb").Wait();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var repository = serviceProvider.GetService<MongoRepository<ModelTest>>();
        await repository!.InsertAsync(new ModelTest() { Name = "hello" });
        var res=await repository.GetPaginatedAsync(it => true);
        Assert.NotNull(res);
    }
    internal class ModelTest:MongoEntity
    {
        public string Name { get; set; }
    }
}