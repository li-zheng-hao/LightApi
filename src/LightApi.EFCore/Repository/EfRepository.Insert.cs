using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

public  partial class EfRepository<TEntity> : IEfRepository<TEntity>
    where TEntity : class, IEfEntity, new() 
{
    public void Add(TEntity entity)
    {
        DbContext.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbContext.AddRange(entities);
    }
}