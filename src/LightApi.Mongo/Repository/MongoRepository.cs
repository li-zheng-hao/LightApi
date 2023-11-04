#if NET6_0_OR_GREATER
using System.Linq.Expressions;
using FB.Infrastructure.Mongo;
using LightApi.Mongo.Entities;
using LightApi.Mongo.UnitOfWork;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Entities;
using Transaction = MongoDB.Entities.Transaction;

namespace LightApi.Mongo.Repository;

/// <summary>
/// 仓储层基类，一些公用的方法添加在这里
/// </summary>
/// <typeparam name="TDocument"></typeparam>
public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : MongoEntity
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IMongoUnitOfWork _mongoUnitOfWork;

    public Transaction? Transaction;


    public MongoRepository(IServiceProvider serviceProvider, IMongoUnitOfWork mongoUnitOfWork)
    {
        _serviceProvider = serviceProvider;
        _mongoUnitOfWork = mongoUnitOfWork;
        Transaction = mongoUnitOfWork?.MongoTransaction;
    }

    public IMongoRepository<T> ChangeRepository<T>() where T : MongoEntity
    {
        return _serviceProvider.GetService<IMongoRepository<T>>();
    }

    public async Task<TOther> FindNavigation<TOther>(TDocument document, Func<TDocument, string> relationKey, bool ignoreSoftDelete = false) where TOther : IEntity
    {
        var id = relationKey(document);

        if (string.IsNullOrWhiteSpace(id))
            return default;

        if (typeof(TOther).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var isDeletedFilter = Builders<TOther>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);

            return await DB.Find<TOther>().Match(isDeletedFilter).OneAsync(id);
        }

        return await DB.Find<TOther>(Transaction?.Session).MatchID(id).ExecuteSingleAsync();
    }

    public async Task<List<TOther>> FindNavigation<TOther>(TDocument document, Func<TDocument, List<string>> relationKey, bool ignoreSoftDelete = false) where TOther : IEntity
    {
        var ids = relationKey(document);

        if (!ids.Any())
            return default;

        if (typeof(TOther).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var isDeletedFilter = Builders<TOther>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);

            return await DB.Find<TOther>().Match(isDeletedFilter).ManyAsync(it => ids.Contains(it.ID));
        }

        return await DB.Find<TOther>().ManyAsync(it => ids.Contains(it.ID));
    }

    public virtual async Task<int> GetCountAsync(Expression<Func<TDocument, bool>> filter = null, bool ignoreSoftDelete = false)
    {
        FilterDefinition<TDocument> targetFilter = filter ?? Builders<TDocument>.Filter.Where(it => true);

        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            // 检查传入的 filter 是否包含 IsDeleted=true 的过滤条件
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);
            var hasIsDeletedFilter = filter?.Body.ToString().Contains(nameof(ISoftDelete.IsDeleted));

            // 如果没有，则添加额外的过滤条件
            if (!hasIsDeletedFilter.HasValue || !hasIsDeletedFilter.Value)
            {
                targetFilter = Builders<TDocument>.Filter.And(targetFilter, isDeletedFilter);
            }
        }

        var count = await DB.Collection<TDocument>().CountDocumentsAsync(targetFilter);

        return (int)count;
    }

    public virtual async Task<List<TDocument>> GetByIdAsync(IEnumerable<string> ids, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);

            return await DB.Find<TDocument>(Transaction?.Session).Match(filter => filter.In(
                it => it.ID, ids)).Match(isDeletedFilter).ExecuteAsync();
        }
        else
        {
            return await DB.Find<TDocument>(Transaction?.Session).Match(filter => filter.In(
                it => it.ID, ids)).ExecuteAsync();
        }
    }

    public virtual Task<List<T>> GetByIdAsync<T>(IEnumerable<string> ids, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);

            return DB.Find<TDocument, T>(Transaction?.Session).Match(filter => filter.In(
                it => it.ID, ids)).Match(isDeletedFilter).ExecuteAsync();
        }

        return DB.Find<TDocument, T>(Transaction?.Session).Match(filter => filter.In(
            it => it.ID, ids)).ExecuteAsync();
    }

    public virtual async Task<TDocument> GetByIdAsync(string id, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);

            return await DB.Find<TDocument>(Transaction?.Session).Match(isDeletedFilter).OneAsync(id);
        }

        return await DB.Find<TDocument>(Transaction?.Session).OneAsync(id);
    }

    public virtual Task<T> GetByIdAsync<T>(string id, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);

            return DB.Find<TDocument, T>(Transaction?.Session).Match(isDeletedFilter).OneAsync(id);
        }

        return DB.Find<TDocument, T>(Transaction?.Session).OneAsync(id);
    }

    public virtual async Task<TDocument> GetFirstAsync(Expression<Func<TDocument, bool>>? filter = null,
        Expression<Func<TDocument, object>>? orderBy = null, Order? orderType = null, bool ignoreSoftDelete = false)
    {
        FilterDefinition<TDocument> targetFilter = filter;

        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            // 检查传入的 filter 是否包含 IsDeleted=true 的过滤条件
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);
            var hasIsDeletedFilter = filter.Body.ToString().Contains(nameof(ISoftDelete.IsDeleted));

            // 如果没有，则添加额外的过滤条件
            if (!hasIsDeletedFilter)
            {
                targetFilter = Builders<TDocument>.Filter.And(filter, isDeletedFilter);
            }
        }

        var pipeline = DB.Find<TDocument>(Transaction?.Session);

        if (targetFilter != null)
            pipeline.Match(targetFilter);

        if (orderBy != null)
            pipeline.Sort(orderBy, orderType!.Value);

        return await pipeline.ExecuteFirstAsync();
    }

    public virtual async Task<(List<TDocument> Result, int Count)> GetPaginatedAsync(Expression<Func<TDocument, bool>> filter,
        int pageNumber = 1, int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false)
    {
        FilterDefinition<TDocument> targetFilter = filter;

        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            // 检查传入的 filter 是否包含 IsDeleted=true 的过滤条件
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);
            var hasIsDeletedFilter = filter.Body.ToString().Contains(nameof(ISoftDelete.IsDeleted));

            // 如果没有，则添加额外的过滤条件
            if (!hasIsDeletedFilter)
            {
                targetFilter = Builders<TDocument>.Filter.And(filter, isDeletedFilter);
            }
        }

        var pipeline = DB.PagedSearch<TDocument>(Transaction?.Session).Match(targetFilter);

        if (option != null)
        {
            pipeline.Option(option);
        }

        var res = await pipeline.Sort(b => b.ID, Order.Ascending).PageSize(pageSize).PageNumber(pageNumber)
            .ExecuteAsync();

        return (res.Results.ToList(), (int)res.TotalCount);
    }

    public virtual async Task<(List<TDocument> Result, int Count)> GetPaginatedAsync(Expression<Func<TDocument, bool>> filter
        , Expression<Func<TDocument, object>>? orderBy, Order orderType, int pageNumber = 1,
        int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false)
    {
        FilterDefinition<TDocument> targetFilter = filter;

        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            // 检查传入的 filter 是否包含 IsDeleted=true 的过滤条件
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);
            var hasIsDeletedFilter = filter.Body.ToString().Contains(nameof(ISoftDelete.IsDeleted));

            // 如果没有，则添加额外的过滤条件
            if (!hasIsDeletedFilter)
            {
                targetFilter = Builders<TDocument>.Filter.And(filter, isDeletedFilter);
            }
        }

        var pipeline = DB.PagedSearch<TDocument>(Transaction?.Session).Match(targetFilter);

        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }

        var res = await pipeline.PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();

        return (res.Results.ToList(), (int)res.TotalCount);
    }

    public virtual async Task<(List<T> Result, int Count)> GetPaginatedAsync<T>(Expression<Func<TDocument, bool>> filter,
        Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending,
        int pageNumber = 1, int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false)
    {
        FilterDefinition<TDocument> targetFilter = filter;

        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            // 检查传入的 filter 是否包含 IsDeleted=true 的过滤条件
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, false);
            var hasIsDeletedFilter = filter.Body.ToString().Contains(nameof(ISoftDelete.IsDeleted));

            // 如果没有，则添加额外的过滤条件
            if (!hasIsDeletedFilter)
            {
                targetFilter = Builders<TDocument>.Filter.And(filter, isDeletedFilter);
            }
        }

        var pipeline = DB.PagedSearch<TDocument, T>(Transaction?.Session).Match(targetFilter);

        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }

        var res = await pipeline.ProjectExcluding(_ => new
            {
                _._dummyProp
            }).PageSize(pageSize).PageNumber(pageNumber)
            .ExecuteAsync();

        return (res.Results.ToList(), (int)res.TotalCount);
    }

    public virtual async Task<(List<TDocument>, int)> GetPaginatedAsync(FilterDefinition<TDocument> filter,
        Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending, int pageNumber = 1,
        int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<TDocument>(), BsonSerializer.SerializerRegistry)
                .Contains(nameof(ISoftDelete.IsDeleted)) && !ignoreSoftDelete)
        {
            filter &= DB.Filter<TDocument>().Eq(it => ((ISoftDelete)it).IsDeleted, false);
        }

        var pipeline = DB.PagedSearch<TDocument>(Transaction?.Session).Match(filter);

        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }

        var res = await pipeline.PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();

        return (res.Results.ToList(), (int)res.TotalCount);
    }

    public virtual async Task<(List<T> Result, int Count)> GetPaginatedAsync<T>(FilterDefinition<TDocument> filter,
        Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending,
        int pageNumber = 1, int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<TDocument>(), BsonSerializer.SerializerRegistry)
                .Contains(nameof(ISoftDelete.IsDeleted)) && !ignoreSoftDelete)
        {
            filter &= DB.Filter<TDocument>().Eq(it => ((ISoftDelete)it).IsDeleted, false);
        }

        var pipeline = DB.PagedSearch<TDocument, T>(Transaction?.Session).Match(filter);

        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }

        var res = await pipeline.ProjectExcluding(_ => new
            {
                _._dummyProp
            }).PageSize(pageSize).PageNumber(pageNumber)
            .ExecuteAsync();

        return (res.Results.ToList().Adapt<List<T>>(), (int)res.TotalCount);
    }

    public virtual async Task<List<TDocument>> QueryAsync(Expression<Func<TDocument, bool>> filter, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            // 检查传入的 filter 是否包含 IsDeleted=true 的过滤条件
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, true);
            var hasIsDeletedFilter = filter?.Body.ToString().Contains(nameof(ISoftDelete.IsDeleted));

            // 如果没有，则添加额外的过滤条件
            if (hasIsDeletedFilter.HasValue && hasIsDeletedFilter.Value)
                return await DB.Find<TDocument>(Transaction?.Session).Match(filter).ExecuteAsync();

            isDeletedFilter = Builders<TDocument>.Filter.Not(isDeletedFilter);
            var targetFilter = filter == null ? isDeletedFilter : Builders<TDocument>.Filter.And(filter, isDeletedFilter);
            var res = await DB.Find<TDocument>(Transaction?.Session).Match(targetFilter).ExecuteAsync();

            return res;
        }

        return await DB.Find<TDocument>(Transaction?.Session).Match(filter).ExecuteAsync();
    }

    public virtual async Task<List<T>> QueryAsync<T>(Expression<Func<TDocument, bool>> filter, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            // 检查传入的 filter 是否包含 IsDeleted=true 的过滤条件
            var isDeletedFilter = Builders<TDocument>.Filter.Eq(x => ((ISoftDelete)x).IsDeleted, true);
            var hasIsDeletedFilter = filter?.Body.ToString().Contains(nameof(ISoftDelete.IsDeleted));

            // 如果没有，则添加额外的过滤条件
            if (hasIsDeletedFilter.HasValue && hasIsDeletedFilter.Value)
                return await DB.Find<TDocument, T>(Transaction?.Session).Match(filter).ExecuteAsync();

            isDeletedFilter = Builders<TDocument>.Filter.Not(isDeletedFilter);
            var targetFilter = filter == null ? isDeletedFilter : Builders<TDocument>.Filter.And(filter, isDeletedFilter);
            var res = await DB.Find<TDocument, T>(Transaction?.Session).Match(targetFilter).ExecuteAsync();

            return res;
        }

        return await DB.Find<TDocument, T>(Transaction?.Session).Match(filter).ExecuteAsync();
    }

    public virtual async Task<List<T>> QueryAsync<T>(FilterDefinition<TDocument> filter, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<TDocument>(), BsonSerializer.SerializerRegistry)
                .Contains(nameof(ISoftDelete.IsDeleted)) && !ignoreSoftDelete)
        {
            filter &= DB.Filter<TDocument>().Eq(it => ((ISoftDelete)it).IsDeleted, false);
        }

        var res = await DB.Find<TDocument, T>(Transaction?.Session).Match(filter).ExecuteAsync();

        return res;
    }

    public virtual async Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> filter, bool ignoreSoftDelete = false)
    {
        // check filter has IsDeleted field
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<TDocument>(), BsonSerializer.SerializerRegistry)
                .Contains(nameof(ISoftDelete.IsDeleted)) && !ignoreSoftDelete)
        {
            filter &= DB.Filter<TDocument>().Eq(it => ((ISoftDelete)it).IsDeleted, false);
        }

        var res = await DB.Find<TDocument>(Transaction?.Session).Match(filter).ExecuteAsync();

        return res;
    }

    public virtual async Task<ulong> GetNextSequenceNumberAsync(string sequenceName)
    {
        return await DB.NextSequentialNumberAsync(sequenceName);
    }


    public virtual async Task<bool> UpdateAsync(TDocument modifiedDocument)
    {
        // var res=await DB.Find<TDocument>().Match(it=>it.ID==modifiedDocument.ID).ExecuteAsync();
        var res = await DB.Replace<TDocument>(Transaction?.Session).MatchID(modifiedDocument.ID)
            .WithEntity(modifiedDocument).ExecuteAsync();

        return true;
    }

    public async Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, TDocument entity)
    {
        var res = await DB.Update<TDocument>(Transaction?.Session).MatchID(entity.ID).ModifyOnly(members, entity)
            .ExecuteAsync();

        return res.MatchedCount > 0;
    }

    public async Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, List<TDocument> entities)
    {
        if (entities == null || !entities.Any())
            return true;

        var res = await DB.SaveOnlyAsync(entities, members, Transaction?.Session);

        return res.MatchedCount > 0;
    }

    public virtual async Task<bool> UpdateAsync(string id,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(Transaction?.Session).MatchID(id).Modify(operation).ExecuteAsync();

        return true;
    }

    public virtual async Task<bool> UpdateAsync(TDocument documentToModify,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(Transaction?.Session).MatchID(documentToModify.ID).Modify(operation)
            .ExecuteAsync();

        return true;
    }

    public virtual async Task<bool> UpdateAsync(IEnumerable<TDocument> documentsToModify)
    {
        if (documentsToModify != null && documentsToModify.Any())
        {
            var res = await documentsToModify.SaveAsync(Transaction?.Session);

            return res.IsAcknowledged;
        }

        return true;
    }

    public virtual async Task<bool> UpdateAsync(FilterDefinition<TDocument> filter,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(Transaction?.Session).Match(filter).Modify(operation).ExecuteAsync();

        return true;
    }

    public virtual async Task<bool> UpdateAsync(Expression<Func<TDocument, bool>> filter,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(Transaction?.Session).Match(filter).Modify(operation).ExecuteAsync();

        return true;
    }

    public virtual async Task<bool> UpdateAsync(Expression<Func<TDocument, bool>> filter,
        UpdateDefinition<TDocument> updateDefinition)
    {
        var res = await DB.Update<TDocument>(Transaction?.Session).Match(filter).Modify(it => updateDefinition)
            .ExecuteAsync();

        return true;
    }


    public virtual async Task<bool> DeleteAsync(FilterDefinition<TDocument> filter, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var res = await DB.Update<TDocument>(Transaction?.Session).Match(filter).Modify(it => it.Set(it => ((ISoftDelete)it).IsDeleted, true)).ExecuteAsync();

            return res.MatchedCount > 0;
        }
        else
        {
            var res = await DB.DeleteAsync(filter, Transaction?.Session);

            return res.IsAcknowledged && res.DeletedCount > 0;
        }
    }

    public virtual async Task<bool> DeleteAsync(Expression<Func<TDocument, bool>> filter, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var res = await DB.Update<TDocument>(Transaction?.Session)
                .Match(filter)
                .Modify(it =>
                    it.Set(it => ((ISoftDelete)it).IsDeleted, true)
                        .Set(it => ((ISoftDelete)it).DeletedTime, DateTime.Now))
                .ExecuteAsync();

            return res.MatchedCount > 0;
        }
        else
        {
            var res = await DB.DeleteAsync(filter, Transaction?.Session);

            return res.IsAcknowledged && res.DeletedCount > 0;
        }
    }

    public virtual async Task<bool> DeleteAsync(
        Func<FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> filter, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var res = await DB.Update<TDocument>(Transaction?.Session)
                .Match(filter)
                .Modify(it =>
                    it.Set(it => ((ISoftDelete)it).IsDeleted, true)
                        .Set(it => ((ISoftDelete)it).DeletedTime, DateTime.Now))
                .ExecuteAsync();

            return res.IsAcknowledged && res.MatchedCount > 0;
        }
        else
        {
            var res = await DB.DeleteAsync(filter, Transaction?.Session);

            return res.IsAcknowledged && res.DeletedCount > 0;
        }
    }

    public virtual async Task<bool> DeleteAsync(string id, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var res = await DB.Update<TDocument>(Transaction?.Session)
                .MatchID(id)
                .Modify(it => it
                    .Set(it => ((ISoftDelete)it).IsDeleted, true)
                    .Set(it => ((ISoftDelete)it).DeletedTime, DateTime.Now))
                .ExecuteAsync();

            return res.IsAcknowledged && res.MatchedCount > 0;
        }
        else
        {
            var res = await DB.DeleteAsync<TDocument>(id, Transaction?.Session);

            return res.IsAcknowledged && res.DeletedCount > 0;
        }
    }

    public async Task<bool> DeleteAsync(TDocument document, bool ignoreSoftDelete = false)
    {
        if (typeof(TDocument).IsAssignableTo(typeof(ISoftDelete)) && !ignoreSoftDelete)
        {
            var res = await DB.Update<TDocument>(Transaction?.Session).MatchID(document.ID).Modify(it => it.Set(it => ((ISoftDelete)it).IsDeleted, true)
                .Set(it => ((ISoftDelete)it).DeletedTime, DateTime.Now)).ExecuteAsync();

            return res.IsAcknowledged && res.MatchedCount > 0;
        }
        else
        {
            var res = await DB.DeleteAsync<TDocument>(document.ID, Transaction?.Session);

            return res.IsAcknowledged && res.DeletedCount > 0;
        }
    }

    public virtual async Task InsertAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        await DB.SaveAsync(document, Transaction?.Session, cancellationToken);
    }

    public virtual async Task InsertAsync(IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default)
    {
        if (documents != null && documents.Any())
        {
            await DB.SaveAsync(documents, Transaction?.Session, cancellationToken);
        }
    }
}
#endif