#if NET6_0_OR_GREATER

using System.Linq.Expressions;
using LightApi.Mongo.Entities;
using MongoDB.Driver;
using MongoDB.Entities;

namespace LightApi.Mongo.Repository
{
    public interface IMongoRepository<TDocument> where TDocument : MongoEntity
    {
        /// <summary>
        /// 切换仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IMongoRepository<T> ChangeRepository<T>() where T : MongoEntity;

        #region 导航查询

        /// <summary>
        /// 导航查询 (只能通过id查询) 该id为空时返回null
        /// </summary>
        /// <param name="document"></param>
        /// <param name="relationKey"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        Task<TOther> FindNavigation<TOther>(TDocument document, Func<TDocument, string> relationKey, bool ignoreSoftDelete = false) where TOther : IEntity;

        /// <summary>
        /// 导航查询 (只能通过id查询) 当该列表为空时返回null
        /// </summary>
        /// <param name="document"></param>
        /// <param name="relationKey"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        Task<List<TOther>> FindNavigation<TOther>(TDocument document, Func<TDocument, List<string>> relationKey, bool ignoreSoftDelete = false) where TOther : IEntity;

        #endregion

        #region 查询

        /// <summary>
        /// 根据条件查询数量
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<int> GetCountAsync(Expression<Func<TDocument, bool>> filter = null, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据id字段批量获取文档
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<List<TDocument>> GetByIdAsync(IEnumerable<string> ids, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据id字段批量获取文档 DTO类
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<List<T>> GetByIdAsync<T>(IEnumerable<string> ids, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据ID字段获取文档
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<TDocument> GetByIdAsync(string id, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据ID字段获取文档 DTO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<T> GetByIdAsync<T>(string id, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据条件获取第一个数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<TDocument> GetFirstAsync(Expression<Func<TDocument, bool>>? filter = null,
            Expression<Func<TDocument, object>>? orderBy = null, Order? orderType = null, bool ignoreSoftDelete = false);

        /// <summary>
        /// 异步分页查询
        /// </summary>
        /// <typeparam name="TDocument">实体类型</typeparam>
        /// <param name="filter">过滤条件</param>
        /// <param name="pageNumber">索引[1,MAX]</param>
        /// <param name="pageSize">页数大小</param>
        /// <param name="option">mongo查询Option</param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        Task<(List<TDocument> Result, int Count)> GetPaginatedAsync(Expression<Func<TDocument, bool>> filter, int pageNumber = 1,
            int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false
        );

        /// <summary>
        /// 异步分页查询
        /// </summary>
        /// <typeparam name="TDocument">实体类型</typeparam>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="option"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        Task<(List<TDocument> Result, int Count)> GetPaginatedAsync(Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>>? orderBy, Order orderType = Order.Descending, int pageNumber = 1,
            int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false);

        /// <summary>
        /// 异步分页查询
        /// </summary>
        /// <typeparam name="TDocument">实体类型</typeparam>
        /// <typeparam name="T">DTO类</typeparam>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="option"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        Task<(List<T> Result, int Count)> GetPaginatedAsync<T>(Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> orderBy = null, Order orderType = Order.Descending, int pageNumber = 1,
            int pageSize = 50, Action<AggregateOptions> option = null, bool ignoreSoftDelete = false);

        /// <summary>
        /// 异步分页查询 默认只查询前50条
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="option"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<(List<TDocument> Result, int Count)> GetPaginatedAsync(FilterDefinition<TDocument> filter,
            Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending, int pageNumber = 1,
            int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false);

       
        /// <summary>
        /// 异步分页查询 默认只查询前50条
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="option"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <typeparam name="T">DTO类型</typeparam>
        /// <returns></returns>
        Task<(List<T> Result, int Count)> GetPaginatedAsync<T>(FilterDefinition<TDocument> filter,
            Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending, int pageNumber = 1,
            int pageSize = 50, Action<AggregateOptions>? option = null, bool ignoreSoftDelete = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<List<TDocument>> QueryAsync(Expression<Func<TDocument, bool>> filter, bool ignoreSoftDelete = false);

        /// <summary>
        /// T为DTO类
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<TDocument, bool>> filter, bool ignoreSoftDelete = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> filter, bool ignoreSoftDelete = false);

        /// <summary>
        /// T为DTO类
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(FilterDefinition<TDocument> filter, bool ignoreSoftDelete = false);

        /// <summary>
        /// 获取一个从1开始的序列号 原子操作
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        Task<ulong> GetNextSequenceNumberAsync(string sequenceName);

        #endregion

        #region 更新

        /// <summary>
        /// 根据Id更新文档
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
        Task<bool> UpdateAsync(TDocument modifiedDocument);

        /// <summary>
        /// 根据Id更新文档
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
        Task<bool> UpdateAsync(string id,
            Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

        /// <summary>
        /// 根据更新条件更新一个文档
        /// </summary>
        /// <param name="documentToModify"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(TDocument documentToModify,
            Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

        /// <summary>
        /// 只更新某些字段
        /// </summary>
        /// <param name="members">必须使用 it=>new { it.A, it.B} 格式! , it=>it.A 无效</param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, TDocument entity);

        /// <summary>
        /// 只更新某些字段
        /// </summary>
        /// <param name="members">必须使用 it=>new { it.A, it.B} 格式! , it=>it.A 无效</param>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, List<TDocument> entities);


        /// <summary>
        /// 根据主键批量更新多个文件
        /// </summary>
        /// <param name="documentsToModify"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(IEnumerable<TDocument> documentsToModify);

        /// <summary>
        /// 根据更新条件更新多个文档
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="updateDefinition"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        Task<bool> UpdateAsync(FilterDefinition<TDocument> filter,
            Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

        /// <summary>
        /// 根据更新条件更新多个文档
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="updateDefinition"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        Task<bool> UpdateAsync(Expression<Func<TDocument, bool>> filter,
            Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

        /// <summary>
        /// 根据更新条件更新多个文档
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="updateDefinition"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        Task<bool> UpdateAsync(Expression<Func<TDocument, bool>> filter,
            UpdateDefinition<TDocument> updateDefinition);

        #endregion

        #region 删除

        /// <summary>
        /// 根据条件删除文档
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(FilterDefinition<TDocument> filter, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据条件删除文档
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Expression<Func<TDocument, bool>> filter, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据条件删除文档
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Func<FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> filter, bool ignoreSoftDelete = false);

        /// <summary>
        /// 根据id删除文档
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string id, bool ignoreSoftDelete = false);

        /// <summary>
        /// 删除一个文档
        /// </summary>
        /// <param name="document"></param>
        /// <param name="ignoreSoftDelete">是否忽略软删除标志位</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(TDocument document, bool ignoreSoftDelete = false);

        #endregion

        #region 新增

        /// <summary>
        /// 新增文档
        /// </summary>
        /// <param name="document"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InsertAsync(TDocument document, CancellationToken cancellationToken = default);

        /// <summary>
        /// 新增多个文档
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InsertAsync(IEnumerable<TDocument> documents,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
#endif