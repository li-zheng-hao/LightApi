using System.Reflection;
using LightApi.EFCore;
using LightApi.EFCore.Config;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Interceptors;
using LightApi.EFCore.MySql.Transaction;
using LightApi.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureEfCoreMySql(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsBuilder,
        Assembly entityAssembly = null
    )
    {
        if (entityAssembly is null)
            entityAssembly = Assembly.GetEntryAssembly();

        var serviceType = typeof(IEntityInfo);
        var implType = entityAssembly.ExportedTypes.FirstOrDefault(
            type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true)
        );

        if (implType is null)
            throw new NotImplementedException(
                $"模型所在程序集必须继承{nameof(IEntityInfo)}接口,或者直接派生{nameof(AbstractSharedEntityInfo)}类"
            );
        else
            services.AddSingleton(serviceType, implType);

        if (services.HasRegistered(nameof(AddInfrastructureEfCoreMySql)))
            return services;

        services.TryAddScoped<IUnitOfWork, MySqlUnitOfWork<AppDbContext>>();

        services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));

        services.AddDbContext<AppDbContext>(
            (sp, op) =>
            {
                optionsBuilder(op);
                op.AddInterceptors(new SoftDeleteInterceptor());
            }
        );

        return services;
    }

    public static IServiceCollection AddInfrastructureEfCoreMySql<TAppContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsBuilder,
        Assembly entityAssembly = null
    )
        where TAppContext : AppDbContext
    {
        if (entityAssembly is null)
            entityAssembly = Assembly.GetExecutingAssembly();

        var serviceType = typeof(IEntityInfo);
        var implType = entityAssembly.ExportedTypes.FirstOrDefault(
            type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true)
        );

        if (implType is null)
            throw new NotImplementedException(
                $"模型所在程序集必须继承{nameof(IEntityInfo)}接口,或者直接派生{nameof(AbstractSharedEntityInfo)}类"
            );
        else
            services.AddSingleton(serviceType, implType);

        if (services.HasRegistered(nameof(AddInfrastructureEfCoreMySql)))
            return services;

        services.AddScoped<AppDbContext>(sp => sp.GetService<TAppContext>());

        services.TryAddScoped<IUnitOfWork, MySqlUnitOfWork<TAppContext>>();

        services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));

        services.AddDbContext<TAppContext>(
            (sp, op) =>
            {
                optionsBuilder(op);
                op.AddInterceptors(new SoftDeleteInterceptor());
            }
        );

        return services;
    }
}
