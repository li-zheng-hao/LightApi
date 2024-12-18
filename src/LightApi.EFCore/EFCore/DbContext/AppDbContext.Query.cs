using LightApi.EFCore.Entities;

namespace LightApi.EFCore.EFCore.DbContext;

public partial class AppDbContext
{
    public virtual DbSet<TEntity> Entities<TEntity>()
        where TEntity : class, IEfEntity
    {
        return Set<TEntity>();
    }

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <param name="tracking">是否跟踪实体,默认不跟踪</param>
    /// <returns>IQueryable{TEntity}</returns>
    public virtual IQueryable<TEntity> AsQueryable<TEntity>(bool tracking = false)
        where TEntity : class, IEfEntity
    {
        var entities = Entities<TEntity>();

        return !tracking ? entities.AsNoTracking() : entities.AsQueryable();
    }
}
