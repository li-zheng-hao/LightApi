namespace LightApi.EFCore.Repository;

public interface ISqlQueryRepository
{
    /// <summary>
    /// 使用Dapper进行查询
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="timeout"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters=null,int timeout=30);
    
    /// <summary>
    /// 使用Dapper进行单个查询
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="timeout"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> QuerySingleAsync<T>(string sql, object parameters=null,int timeout=30);

}