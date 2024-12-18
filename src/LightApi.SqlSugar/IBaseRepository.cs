using SqlSugar;

namespace LightApi.SqlSugar;

public interface IBaseRepository<T> : ISugarRepository, ISimpleClient<T>
    where T : class, new() { }
