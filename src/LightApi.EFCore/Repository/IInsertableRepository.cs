using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

public interface IInsertableRepository<TEntity> where TEntity : class, IEfEntity
{
    void Add(TEntity entity);
    
    void AddRange(IEnumerable<TEntity> entities);

}