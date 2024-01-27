using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IInsertableRepository<TEntity> where TEntity : class,IEfEntity
{
    void Add(TEntity entity);
    
    void AddRange(IEnumerable<TEntity> entities);

}