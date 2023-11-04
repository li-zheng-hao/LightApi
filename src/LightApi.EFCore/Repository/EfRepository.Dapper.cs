using Dapper;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Repository
{
    /// <summary>
    /// Ef默认的、全功能的仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class EfRepository<TEntity>: ISqlQueryRepository
        where TEntity : class, IEfEntity, new()
    {
     
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, int timeout = 30)
        {
            return DbContext.Database.GetDbConnection().QueryAsync<T>(sql, parameters, DbContext.Database.CurrentTransaction?.GetDbTransaction(), timeout);
        }

        public Task<T> QuerySingleAsync<T>(string sql, object parameters = null, int timeout = 30)
        {
            return DbContext.Database.GetDbConnection().QuerySingleAsync<T>(sql, parameters, DbContext.Database.CurrentTransaction?.GetDbTransaction(), timeout);
        }


      
    }
}