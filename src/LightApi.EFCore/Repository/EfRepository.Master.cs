using System.Data.Common;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Util;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.EFCore.Repository;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public partial class EfRepository<TEntity> : IEfRepository<TEntity>
    where TEntity : class, IEfEntity, new()
{
    public virtual AppDbContext DbContext { get; set; }

    /// <summary>
    /// 数据库操作对象
    /// </summary>
    public virtual DatabaseFacade Database { get; }

    public virtual DbSet<TEntity> Entities { get; }

    public virtual IQueryable<TEntity> DetachedEntities { get; }

    public virtual DbConnection DbConnection { get; }


    public virtual IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public EfRepository(AppDbContext dbContext, IServiceProvider serviceProvider)
    {
        DbContext = dbContext;
        Database = dbContext.Database;
        ServiceProvider = serviceProvider;
        //初始化实体
        Entities = dbContext.Set<TEntity>();
        DetachedEntities = Entities.AsNoTracking();

        // 只有关系型数据库才有连接信息
        if (dbContext.Database.IsRelational()) DbConnection = dbContext.Database.GetDbConnection();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEfRepository<T> Change<T>() where T : class, IEfEntity
    {
        return ServiceProvider.GetService<IEfRepository<T>>()!;
    }
    

    public IQueryable<TEntity> AsQueryable(bool tracking = false)
    {
        return DbContext.AsQueryable<TEntity>(tracking);
    }

    public IQueryable<OtherEntity> AsQueryable<OtherEntity>(bool tracking = false)
        where OtherEntity : class, IEfEntity
    {
        return DbContext.AsQueryable<OtherEntity>(tracking);
    }

    public DbSet<T> GetDbSet<T>() where T : class, IEfEntity
    {
        return DbContext.Set<T>();
    }

    public DbSet<TEntity> GetDbSet()
    {
        return DbContext.Set<TEntity>();
    }

    public Task SaveChangesAsync()
    {
        return DbContext.SaveChangesAsync();
    }

    public void SaveChanges()
    {
        DbContext.SaveChanges();
    }

    public void Add(object entity)
    {
        DbContext.Add(entity);
    }

    public void AddRange(IEnumerable<object> entities)
    {
        DbContext.AddRange(entities);
    }

    public void Attach(object entity)
    {
        DbContext.Attach(entity);
    }

    public void AttachRange(IEnumerable<object> entities)
    {
        DbContext.AttachRange(entities);
    }

    public void Detach(object entity)
    {
        DbContext.Entry(entity).State = EntityState.Detached;
    }

    public void DetachRange(IEnumerable<object> entities)
    {
        foreach (var entity in entities)
        {
            DbContext.Entry(entity).State = EntityState.Detached;
        }
    }

    public void ClearAllTracks()
    {
        DbContext.ChangeTracker.Clear();
    }
}