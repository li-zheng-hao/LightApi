using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IDeletableRepository<TEntity> where TEntity : class, IEfEntity
{
   /// <summary>
   /// 删除一条
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
   Task<bool> DeleteByKey(object? id);
   
   /// <summary>
   /// 删除 如果不存在则忽略
   /// </summary>
   /// <param name="entity"></param>
   void Remove(object? entity);
   
   /// <summary>
   /// 删除 如果不存在则忽略
   /// </summary>
   /// <param name="entities"></param>
   void RemoveRange(IEnumerable<object>? entities);
}