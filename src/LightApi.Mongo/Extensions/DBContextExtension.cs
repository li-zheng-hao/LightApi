using System.Linq.Expressions;
using LightApi.Mongo.Entities;
using LightApi.Mongo.InternalExceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;

namespace LightApi.Mongo.Extensions;

/// <summary>
/// DBContext扩展功能
/// </summary>
public static class DBContextExtension
{
    public static async Task<UpdateResult> SaveWithOptimisticAsync<T>(this DBContext dbContext, T entity) where T : IOptimisticLock
    {
        // 通过反射获取属性Id的值
        object Id = EntityCache.GetIdValue(entity);
        string oldVersion = entity.Version;
        entity.Version = ObjectId.GenerateNewId().ToString();
        var updateResult = await DB.Update<T>()
            .MatchID(Id)
            .Match(it => it.Version == oldVersion)
            .ModifyWith(entity)
            .ExecuteAsync();

        if (!(updateResult.IsAcknowledged && updateResult.ModifiedCount == 1))
        {
            throw new MongoOptimisticException();
        }
        return updateResult;
    }
    public static async Task<UpdateResult> SaveOnlyWithOptimisticAsync<T>(this DBContext dbContext, T entity, Expression<Func<T, object?>> members) where T : IOptimisticLock
    {
        // 通过反射获取属性Id的值
        object Id = EntityCache.GetIdValue(entity);
        string oldVersion = entity.Version;
        entity.Version = ObjectId.GenerateNewId().ToString();
        var updateResult = await DB.Update<T>()
            .MatchID(Id)
            .Match(it => it.Version == oldVersion)
            .ModifyOnly(members, entity)
            .ExecuteAsync();

        if (!(updateResult.IsAcknowledged && updateResult.ModifiedCount == 1))
        {
            throw new MongoOptimisticException();
        }
        return updateResult;
    }
}
