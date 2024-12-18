#if NET6_0_OR_GREATER
using LightApi.Mongo.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;

namespace LightApi.Mongo;

/// <summary>
///
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// 配置mongodb
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="dbName"></param>
    /// <param name="connectionString"></param>
    /// <param name="immediateConnect">是否立即建立连接，默认true</param>
    /// <returns></returns>
    public static IServiceCollection AddMongoSetup(
        this IServiceCollection serviceCollection,
        string dbName,
        string connectionString,
        bool immediateConnect = true
    )
    {
        if (immediateConnect)
            DB.InitAsync(dbName, MongoClientSettings.FromConnectionString(connectionString))
                .Wait(5000);
        serviceCollection.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
        serviceCollection.AddScoped<DBContext>();
        return serviceCollection;
    }

    /// <summary>
    /// 配置mongodb
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="dbName"></param>
    /// <param name="settings">连接配置</param>
    /// <param name="immediateConnect">是否立即建立连接，默认true</param>
    /// <returns></returns>
    public static IServiceCollection AddMongoSetup(
        this IServiceCollection serviceCollection,
        string dbName,
        MongoClientSettings settings,
        bool immediateConnect = true
    )
    {
        if (immediateConnect)
            DB.InitAsync(dbName, settings).Wait(5000);
        serviceCollection.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
        serviceCollection.AddScoped<DBContext>();
        return serviceCollection;
    }
}
#endif
