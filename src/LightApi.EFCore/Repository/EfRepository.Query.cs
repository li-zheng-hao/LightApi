using System.Reflection;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository
{
    /// <summary>
    /// Ef默认的、全功能的仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class EfRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class, IEfEntity, new()
    {
      private static readonly MethodInfo ContainsMethod = typeof(Enumerable).GetMethods()
            .FirstOrDefault(m => m.Name == "Contains"
                                 && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(object));

        public IQueryable<TEntity> GetById(object id, bool useTracking = false)
        {
            var dbSet = DbContext.Set<TEntity>();

            IQueryable<TEntity> queryable;

            if (useTracking)
                queryable = dbSet.AsTracking();
            else
            {
                queryable = dbSet.AsNoTracking();
            }

            var entityType = dbSet.EntityType;

            var primaryKey = entityType.FindPrimaryKey();

            if (primaryKey!.Properties.Count != 1)
                throw new NotSupportedException("Only a single primary key is supported");

            var pkProperty = primaryKey.Properties[0];
            var pkPropertyType = pkProperty.ClrType;

            // validate passed key value
            if (!pkPropertyType.IsAssignableFrom(id.GetType()))
                throw new ArgumentException($"Key value '{id}' is not of the right type");

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

            return queryable.Where(predicateExpression);
        }

        public IQueryable<TEntity> GetById(IEnumerable<object> ids, bool useTracking = false)
        {
            var dbSet = DbContext.Set<TEntity>();

            IQueryable<TEntity> queryable;

            if (useTracking)
                queryable = dbSet.AsTracking();
            else
            {
                queryable = dbSet.AsNoTracking();
            }

            var entityType = dbSet.EntityType;

            var primaryKey = entityType.FindPrimaryKey();

            if (primaryKey!.Properties.Count != 1)
                throw new NotSupportedException("Only a single primary key is supported");

            var pkProperty = primaryKey.Properties[0];
            var pkPropertyType = pkProperty.ClrType;

            // validate passed key values
            foreach (var keyValue in ids)
            {
                if (!pkPropertyType.IsAssignableFrom(keyValue.GetType()))
                    throw new ArgumentException($"Key value '{keyValue}' is not of the right type");
            }

            // retrieve member info for primary key
            var pkMemberInfo = typeof(TEntity).GetProperty(pkProperty.Name);

            if (pkMemberInfo == null)
                throw new ArgumentException("Type does not contain the primary key as an accessible property");

            // build lambda expression
            var parameter = Expression.Parameter(typeof(TEntity), "e");

            var body = Expression.Call(null, ContainsMethod,
                Expression.Constant(ids),
                Expression.Convert(Expression.MakeMemberAccess(parameter, pkMemberInfo), typeof(object)));

            var predicateExpression = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            return queryable.Where(predicateExpression);
        }

        public IQueryable<TEntity> GetById(List<int> ids, bool useTracking = false)
        {
            List<object> internalIds = new List<object> { ids };
            return GetById(internalIds);
        }

        public Task<TEntity> FirstOrDefaultAsync(bool useFilter,
            Expression<Func<TEntity, bool>> condition,
            bool useTracking = false)
        {
            if (useFilter)
                return FirstOrDefaultAsync(condition, useTracking);

            var queryable = AsQueryable(useTracking);

            return queryable.FirstOrDefaultAsync(condition);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> condition, bool useTracking = false)
        {
            return AsQueryable(useTracking).FirstOrDefaultAsync(condition);
        }

        public IQueryable<TEntity> IncludeAll(bool useTracking = false)
        {
            var queryable = AsQueryable(useTracking);
            var type = typeof(TEntity);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var isVirtual = property.GetGetMethod()!.IsVirtual;

                if (isVirtual)
                {
                    queryable = queryable.Include(property.Name);
                }
            }

            return queryable;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> condition, bool useTracking = false)
        {
            return AsQueryable(useTracking).Where(condition);
        }
    }
}