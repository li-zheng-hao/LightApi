using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Util;

namespace LightApi.EFCore.Repository;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IPageQueryRepository<TEntity> where TEntity : class, IEfEntity
{
    /// <summary>
    /// 分页查询 带排序
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="orderExp"></param>
    /// <param name="isAsc"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    Task<PageList<TEntity>> PageQueryAsync<TKey>(Expression<Func<TEntity, bool>> condition, int pageIndex, int pageSize
        , Expression<Func<TEntity, TKey>> orderExp=null,
        bool isAsc = true);

    /// <summary>
    /// 分页查询 带排序
    /// </summary>
    /// <param name="useFilter">是否启动过滤条件</param>
    /// <param name="condition"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="orderExp"></param>
    /// <param name="isAsc"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    Task<PageList<TEntity>> PageQueryAsync<TKey>(bool useFilter, Expression<Func<TEntity, bool>> condition, int pageIndex, int pageSize,
        Expression<Func<TEntity, TKey>> orderExp = null,
        bool isAsc = true);


    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<PageList<TEntity>> PageQueryAsync(Expression<Func<TEntity, bool>> condition, int pageIndex, int pageSize);
}