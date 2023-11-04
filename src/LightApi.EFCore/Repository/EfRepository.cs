using System.Data.Common;
using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.EFCore.Repository
{
    /// <summary>
    /// Ef默认的、全功能的仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class EfRepository<TEntity> : IEfRepository<TEntity>
        where TEntity : class, IEfEntity, new()
    {
        public virtual LightApi.EFCore.EFCore.DbContext.AppDbContext DbContext { get; }

        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public virtual DatabaseFacade Database { get; }

        public DbSet<TEntity> Entities { get; }

        public IQueryable<TEntity> DetachedEntities { get; }

        public DbConnection DbConnection { get; }

        public IServiceProvider ServiceProvider { get; }

        public EfRepository(LightApi.EFCore.EFCore.DbContext.AppDbContext dbContext, IServiceProvider serviceProvider = null)
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

        public Task SaveChangeAsync()
        {
            return DbContext.SaveChangesAsync();
        }

        public void SaveChange()
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
}