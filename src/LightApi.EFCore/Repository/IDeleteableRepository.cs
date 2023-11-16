using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

public interface IDeleteableRepository<TEntity> where TEntity : class, IEfEntity
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