using Dapper;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.EFCore.DbContext;

public static class AppDbContextExtensions
{
    public static DbSet<TEntity> Entities<TEntity>(this AppDbContext dbContext)
        where TEntity : class, IEfEntity
    {
        return dbContext.Set<TEntity>();
    }

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="tracking">是否跟踪实体,默认不跟踪</param>
    /// <returns>IQueryable{TEntity}</returns>
    public static IQueryable<TEntity> AsQueryable<TEntity>(
        this AppDbContext dbContext,
        bool tracking = false
    )
        where TEntity : class, IEfEntity
    {
        var entities = dbContext.Entities<TEntity>();

        return !tracking ? entities.AsNoTracking() : entities.AsQueryable();
    }

    /// <summary>
    /// 执行Dapper查询
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    public static Task<IEnumerable<T>> DapperQueryAsync<T>(
        this AppDbContext dbContext,
        string sql,
        object? parameters = null,
        int timeout = 30
    )
    {
        return dbContext
            .Database.GetDbConnection()
            .QueryAsync<T>(
                sql,
                parameters,
                dbContext.Database.CurrentTransaction?.GetDbTransaction(),
                timeout
            );
    }

    /// <summary>
    /// 执行Dapper查询单行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static Task<T> DapperQuerySingleAsync<T>(
        this AppDbContext dbContext,
        string sql,
        object? parameters = null,
        int timeout = 30
    )
    {
        return dbContext
            .Database.GetDbConnection()
            .QuerySingleAsync<T>(
                sql,
                parameters,
                dbContext.Database.CurrentTransaction?.GetDbTransaction(),
                timeout
            );
    }

    /// <summary>
    /// 执行Dapper查询第一行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static Task<T?> DapperQueryFirstOrDefaultAsync<T>(
        this AppDbContext dbContext,
        string sql,
        object? parameters = null,
        int timeout = 30
    )
    {
        return dbContext
            .Database.GetDbConnection()
            .QueryFirstOrDefaultAsync<T>(
                sql,
                parameters,
                dbContext.Database.CurrentTransaction?.GetDbTransaction(),
                timeout
            );
    }

    /// <summary>
    /// 执行Dapper执行
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static Task<int> DapperExecuteAsync(
        this AppDbContext dbContext,
        string sql,
        object? parameters = null,
        int timeout = 30
    )
    {
        return dbContext
            .Database.GetDbConnection()
            .ExecuteAsync(
                sql,
                parameters,
                dbContext.Database.CurrentTransaction?.GetDbTransaction(),
                timeout
            );
    }

    /// <summary>
    /// 执行Dapper执行标量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static Task<T?> DapperExecuteScalarAsync<T>(
        this AppDbContext dbContext,
        string sql,
        object? parameters = null,
        int timeout = 30
    )
    {
        return dbContext
            .Database.GetDbConnection()
            .ExecuteScalarAsync<T>(
                sql,
                parameters,
                dbContext.Database.CurrentTransaction?.GetDbTransaction(),
                timeout
            );
    }
}
