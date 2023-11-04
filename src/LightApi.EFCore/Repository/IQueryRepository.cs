using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

public interface IQueryRepository<TEntity> where TEntity : class, IEfEntity
{
    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="id"></param>
    /// <param name="useTracking">是否跟踪</param>
    /// <returns></returns>
    IQueryable<TEntity> GetById(object id, bool useTracking = false);

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="ids">多个id</param>
    /// <param name="useTracking">是否跟踪</param>
    /// <returns></returns>
    IQueryable<TEntity> GetById(IEnumerable<object> ids, bool useTracking = false);

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="ids">多个id</param>
    /// <param name="useTracking">是否跟踪</param>
    /// <returns></returns>
    IQueryable<TEntity> GetById(List<int> ids, bool useTracking = false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="useFilter">是否使用过滤</param>
    /// <param name="condition"></param>
    /// <param name="useTracking">是否开启跟踪 默认false</param>
    /// <returns></returns>
    Task<TEntity?> FirstOrDefaultAsync(bool useFilter, Expression<Func<TEntity, bool>> condition,
        bool useTracking = false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="useTracking">是否开启跟踪 默认false</param>
    /// <returns></returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> condition, bool useTracking = false);

    /// <summary>
    /// 加载所有导航属性 仅限一层深度
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> IncludeAll(bool useTracking = false);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="useTracking"></param>
    /// <returns></returns>
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> condition, bool useTracking = false);
}