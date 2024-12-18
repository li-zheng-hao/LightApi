using SqlSugar;

namespace LightApi.SqlSugar;

public class BaseRepository<T> : SimpleClient<T>, IBaseRepository<T>
    where T : class, new()
{
    public BaseRepository(ISqlSugarClient context = null)
        : base(context) { }
}
