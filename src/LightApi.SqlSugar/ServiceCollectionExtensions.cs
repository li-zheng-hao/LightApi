using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace LightApi.SqlSugar;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSugarRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        return services;
    }
}
