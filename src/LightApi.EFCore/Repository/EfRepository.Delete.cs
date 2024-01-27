using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository;

public  partial class EfRepository<TEntity> : IEfRepository<TEntity>
    where TEntity : class, IEfEntity, new() 
{
    public async Task<bool> DeleteByKey(object? id)
    {
        if(id is null) return false;
        
        if(typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            var entity = await GetById(id).FirstOrDefaultAsync();
            if(entity is null) return false;
            ((ISoftDelete)entity).Delete(); 
            DbContext.Update(entity);
            return true;
        }
        
        if(typeof(ISoftDeleteV2).IsAssignableFrom(typeof(TEntity)))
        {
            var entity = await GetById(id).FirstOrDefaultAsync();
            if(entity is null) return false;
            ((ISoftDeleteV2)entity).Delete(); 
            DbContext.Update(entity);
            return true;
        }
        
        var dbSet = DbContext.Set<TEntity>();

        IQueryable<TEntity> queryable=dbSet.AsQueryable();

        var entityType = dbSet.EntityType;

        var primaryKey = entityType.FindPrimaryKey();

        if (primaryKey!.Properties.Count != 1)
            throw new NotSupportedException("Only a single primary key is supported");

        var pkProperty = primaryKey.Properties[0];
        var pkPropertyType = pkProperty.ClrType;

        // retrieve member info for primary key
        var pkMemberInfo = typeof(TEntity).GetProperty(pkProperty.Name);

        if (pkMemberInfo == null)
            throw new ArgumentException("Type does not contain the primary key as an accessible property");

        // build lambda expression
        var parameter = Expression.Parameter(typeof(TEntity), "e");

        var body = Expression.Equal(
            Expression.MakeMemberAccess(parameter, pkMemberInfo),
            Expression.Constant(id));

        var predicateExpression = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return (await queryable.Where(predicateExpression).ExecuteDeleteAsync())==1;
    }

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