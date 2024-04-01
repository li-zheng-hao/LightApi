using System.Data.Common;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LightApi.EFCore.Repository;

public interface IEfRepository<TEntity> :
    IPageQueryRepository<TEntity>,
    ISqlQueryRepository,
    IQueryRepository<TEntity>,
    IInsertableRepository<TEntity>,
    IDeletableRepository<TEntity>
    where TEntity : class, IEfEntity
{
    /// <summary>
    /// 切换仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEfRepository<T> Change<T>() where T : class, IEfEntity;
    
    /// <summary>
    /// EFCore的DBContext
    /// </summary>
    AppDbContext DbContext { get; set; }
    
    /// <summary>
    /// 数据库操作对象
    /// </summary>
    DatabaseFacade Database { get; }

    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 实体集合
    /// </summary>
    DbSet<TEntity> Entities { get; }

    /// <summary>
    /// 不跟踪的（脱轨）实体
    /// </summary>
    IQueryable<TEntity> DetachedEntities { get; }

    /// <summary>
    /// 数据库连接对象
    /// </summary>
    DbConnection DbConnection { get; }



    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <param name="tracking">是否跟踪实体,默认不跟踪</param>
    /// <returns>IQueryable{TEntity}</returns>
    IQueryable<TEntity> AsQueryable(bool tracking = false);

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <param name="tracking">是否跟踪实体,默认不跟踪</param>
    /// <returns>IQueryable{TEntity}</returns>
    IQueryable<OtherEntity> AsQueryable<OtherEntity>(bool tracking = false) where OtherEntity : class, IEfEntity;


    DbSet<T> GetDbSet<T>() where T : class, IEfEntity;

    DbSet<TEntity> GetDbSet();

    Task<int> SaveChangesAsync();

    int SaveChanges();

    void Add(object entity);

    void AddRange(IEnumerable<object> entities);

    void Attach(object entity);

    void AttachRange(IEnumerable<object> entities);

    /// <summary>
    /// 取消跟踪实体
    /// </summary>
    /// <param name="entity"></param>
    void Detach(object entity);

    /// <summary>
    /// 取消跟踪实体
    /// </summary>
    /// <param name="entities"></param>
    void DetachRange(IEnumerable<object> entities);

    /// <summary>
    /// 清除所有跟踪数据
    /// </summary>
    void ClearAllTracks();
}

/// <summary>
/// 仓储接口
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TDbContext"></typeparam>
public interface IEfRepository<TEntity, out TDbContext> : IEfRepository<TEntity>
    where TEntity : class, IEfEntity
    where TDbContext : AppDbContext
{
    /// <summary>
    /// 切换仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEfRepository<T,TDbContext> Change<T>() where T : class, IEfEntity;
}