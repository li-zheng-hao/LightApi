using System.Data.Common;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.EFCore.Repository
{
    /// <summary>
    /// Ef默认的、全功能的仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class MultiContextEfRepository<TEntity, TDbContext>
        : EfRepository<TEntity>,
            IEfRepository<TEntity, TDbContext>
        where TEntity : class, IEfEntity, new()
        where TDbContext : AppDbContext
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="serviceProvider"></param>
        public MultiContextEfRepository(TDbContext dbContext, IServiceProvider serviceProvider)
            : base(dbContext, serviceProvider) { }

        public IEfRepository<T, TDbContext> Change<T>()
            where T : class, IEfEntity
        {
            return ServiceProvider.GetService<IEfRepository<T, TDbContext>>()!;
        }
    }
}
