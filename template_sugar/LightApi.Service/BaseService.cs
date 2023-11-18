using LightApi.SqlSugar;

namespace LightApi.Service;

public abstract class BaseService<T> where T:class,ISugarTable, new()
{
    public IBaseRepository<T> Repository { get; set; }
}