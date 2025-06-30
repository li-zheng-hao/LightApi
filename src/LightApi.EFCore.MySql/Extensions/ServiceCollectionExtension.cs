using System.Reflection;
using LightApi.EFCore;
using LightApi.EFCore.Config;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Interceptors;
using LightApi.EFCore.Internal;
using LightApi.EFCore.MySql.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 注册EfCore仓储服务
    /// </summary>
    /// <param name="services">服务容器</param>
    /// <param name="optionsBuilder">定义数据库provider及aop等配置</param>
    /// <param name="entityInfoType">
    /// 每个项目必须至少有一个EntityInfo类，继承<see cref="AbstractSharedEntityInfo"/>来实现获取模型类.
    /// 如果是多库的话且不同库使用了不同的表，则需要写多个EntityInfo，并覆盖默认的GetEntityTypes实现，从而实现获取不同的表
    /// </param>
    /// <param name="isMasterDbContext">
    /// 是否为默认的主库对应的上下文，使用了true的这个上下文，也就是默认的主库可以使用IEfRepository[TEntity]仓储，多库的话使用IEfRepository[TEntity,TDbContext]仓储</param>
    /// <typeparam name="TAppContext">上下文类型</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructureEfCoreMysql<TAppContext>(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> optionsBuilder,
        Type entityInfoType,
        bool isMasterDbContext = true
    )
        where TAppContext : AppDbContext
    {
        var serviceType = typeof(IEntityInfo);

        services.AddSingleton(serviceType, entityInfoType);

        services.AddSingleton(entityInfoType);

        if (isMasterDbContext)
        {
            services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<TAppContext>());

            services.TryAddScoped<IUnitOfWork, MySqlUnitOfWork<TAppContext>>();
        }
        Db.AddDbContextModelMap(typeof(TAppContext), entityInfoType);

        Db.AddDbContextUnitOfWorkMap(typeof(TAppContext), typeof(MySqlUnitOfWork<TAppContext>));

        services.AddDbContext<TAppContext>(
            (sp, op) =>
            {
                optionsBuilder(sp, op);
                op.AddInterceptors(new SoftDeleteInterceptor());
            }
        );

        return services;
    }
}
