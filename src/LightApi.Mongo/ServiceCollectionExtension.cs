#if NET6_0_OR_GREATER
using LightApi.Mongo.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;

namespace LightApi.Mongo;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 配置mongodb
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="dbName"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoSetup(this IServiceCollection serviceCollection,string dbName,string connectionString)
    {
        DB.InitAsync(dbName, MongoClientSettings.FromConnectionString(connectionString)).Wait(5000);
        serviceCollection.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
        serviceCollection.AddScoped<DBContext>();
        return serviceCollection;
    }
}
#endif