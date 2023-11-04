using System.Reflection;
using FB.Infrastructure.EFCore.Repository.IRepositories;
using LightApi.EFCore.Config;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;
using LightApi.EFCore.Interceptors;
using LightApi.EFCore.Repository;
using LightApi.EFCore.SqlServer.Transaction;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureEfCoreSqlServer(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder, Assembly entityAssembly = null)
    {
        if (entityAssembly is null)
            entityAssembly = Assembly.GetEntryAssembly();

        var serviceType = typeof(IEntityInfo);
        var implType = entityAssembly.ExportedTypes.FirstOrDefault(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true));

        if (implType is null)
            throw new NotImplementedException($"模型所在程序集必须继承{nameof(IEntityInfo)}接口,或者直接派生{nameof(AbstractSharedEntityInfo)}类");
        else
            services.AddSingleton(serviceType, implType);

        services.TryAddScoped<IUnitOfWork, SqlServerUnitOfWork<AppDbContext>>();

        services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));


        services.AddDbContext<AppDbContext>((sp, op) =>
        {
            optionsBuilder(op);
            op.AddInterceptors(new SoftDeleteInterceptor());
        });

        return services;
    }

    public static IServiceCollection AddInfrastructureEfCoreSqlServer<TAppContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder, Assembly entityAssembly = null)
        where TAppContext : AppDbContext
    {
        if (entityAssembly is null)
            entityAssembly = Assembly.GetExecutingAssembly();

        var serviceType = typeof(IEntityInfo);
        var implType = entityAssembly.ExportedTypes.FirstOrDefault(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true));

        if (implType is null)
            throw new NotImplementedException($"模型所在程序集必须继承{nameof(IEntityInfo)}接口,或者直接派生{nameof(AbstractSharedEntityInfo)}类");
        else
            services.AddSingleton(serviceType, implType);

        services.AddScoped<AppDbContext>(sp=>sp.GetService<TAppContext>());
        
        services.TryAddScoped<IUnitOfWork, SqlServerUnitOfWork<TAppContext>>();

        services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));

        services.AddDbContext<TAppContext>((sp, op) =>
        {
            optionsBuilder(op);
            op.AddInterceptors(new SoftDeleteInterceptor());
        });

        return services;
    }
}