using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

public partial class EfRepository<TEntity> : IDeleteableRepository<TEntity> where TEntity : class, IEfEntity, new()
{
    public void Remove(object? entity)
    {
        if(entity is null) return;
        
        DbContext.Remove(entity);
    }

    public void RemoveRange(IEnumerable<object>? entities)
    {
        
        if(entities.IsNullOrEmpty()) return;
        
        DbContext.RemoveRange(entities);
    }
}