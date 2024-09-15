using LightApi.Common;
using LightApi.Common.Page;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Util;

namespace LightApi.EFCore.Repository;

public  partial class EfRepository<TEntity> : IEfRepository<TEntity>
    where TEntity : class, IEfEntity, new() 
{
     public async Task<PageList<TEntity>> PageQueryAsync<TKey>(Expression<Func<TEntity, bool>> condition, int pageIndex,
        int pageSize, Expression<Func<TEntity, TKey>> orderExp = null, bool isAsc = true)
    {
        var pipeline = DbContext.AsQueryable<TEntity>().Where(condition);

        var count = await pipeline.CountAsync();

        if (orderExp != null)
            pipeline = isAsc ? pipeline.OrderBy(orderExp) : pipeline.OrderByDescending(orderExp);

        var rows=await pipeline.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PageList<TEntity>(rows, pageIndex, pageSize, count); 
    }

    public async Task<PageList<TEntity>> PageQueryAsync<TKey>(bool useFilter, Expression<Func<TEntity, bool>> condition, int pageIndex,
        int pageSize, Expression<Func<TEntity, TKey>> orderExp = null, bool isAsc = true)
    {
        if (useFilter)
            return await PageQueryAsync(condition, pageIndex, pageSize);

        var pipeline = DbContext.AsQueryable<TEntity>().Where(condition);

        var count = await pipeline.CountAsync();

        if (orderExp != null)
            pipeline = isAsc ? pipeline.OrderBy(orderExp) : pipeline.OrderByDescending(orderExp);

        var rows = await pipeline.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PageList<TEntity>(rows, pageIndex, pageSize, count); 
    }

    public async Task<PageList<TEntity>> PageQueryAsync(bool useFilter, Expression<Func<TEntity, bool>> condition, int pageIndex,
        int pageSize)
    {
        if (useFilter)
            return await PageQueryAsync(condition, pageIndex, pageSize);

        var pipeline = DbContext.AsQueryable<TEntity>().Where(condition);

        var count = await pipeline.CountAsync();

        var rows = await pipeline.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PageList<TEntity>(rows, pageIndex, pageSize, count); 
    }

    public async Task<PageList<TEntity>> PageQueryAsync(Expression<Func<TEntity, bool>> condition, int pageIndex, int pageSize)
    {
        var pipeline = DbContext.AsQueryable<TEntity>().Where(condition);

        var count = await pipeline.CountAsync();

        var rows = await pipeline.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PageList<TEntity>(rows, pageIndex, pageSize, count); 
    }
}