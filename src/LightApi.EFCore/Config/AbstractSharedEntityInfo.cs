using System.Reflection;
using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightApi.EFCore.Config;

/// <summary>
/// 用于定义实体信息
/// </summary>
public abstract class AbstractSharedEntityInfo : IEntityInfo
{
    protected AbstractSharedEntityInfo()
    {
    }

    protected abstract Assembly GetCurrentAssembly();

    /// <summary>
    /// 如果是多库且不同库表不同的情况下，需要重新实现这个方法
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    protected virtual IEnumerable<Type> GetEntityTypes(Assembly assembly)
    {
        var typeList = assembly.GetTypes().Where(m =>
            m.FullName != null
            && typeof(IEfEntity).IsAssignableFrom(m)
            && !m.IsAbstract);
        if (typeList is null)
            typeList = new List<Type>();

        return typeList;
    }

    public virtual void OnModelCreating(dynamic modelBuilder)
    {
        if (modelBuilder is not ModelBuilder builder)
            throw new ArgumentNullException(nameof(modelBuilder));

        var entityAssembly = GetCurrentAssembly();
        var assemblies = new List<Assembly> { entityAssembly };

        var entityTypes = GetEntityTypes(entityAssembly);
        entityTypes?.ForEach(t =>
        {
            var typeBuilder = builder.Entity(t);
            ConfigureEntity(typeBuilder, t);
        });

        assemblies?.ForEach(assembly => builder.ApplyConfigurationsFromAssembly(assembly));

        // SetComment(modelBuilder, entityTypes);

        // SetTableName(modelBuilder);
    }

    /// <summary>
    /// 配置实体类  主键自增、软删除
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="entityType"></param>
    protected virtual void ConfigureEntity(EntityTypeBuilder  builder,Type entityType)
    {
        builder.HasKey("Id");

        if (entityType.HasAttribute<NonAutoIncrementAttribute>())
        {
            builder.Property("Id").ValueGeneratedNever();
        }
        if (typeof(ISoftDelete).IsAssignableFrom(entityType))
        {
            Type classType = typeof(AbstractSharedEntityInfo);
            MethodInfo methodInfo = classType.GetMethod("ConfigureSoftDelete")!;

            Type[] genericArguments = new Type[] { entityType };
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(genericArguments);

            genericMethod.Invoke(null, new object[] { builder });
        }
        else if (typeof(ISoftDeleteV2).IsAssignableFrom(entityType))
        {
            Type classType = typeof(AbstractSharedEntityInfo);
            MethodInfo methodInfo = classType.GetMethod("ConfigureSoftDeleteV2")!;

            Type[] genericArguments = new Type[] { entityType };
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(genericArguments);

            genericMethod.Invoke(null, new object[] { builder });
        }
    }
    
    public static void ConfigureSoftDelete<TEntity>(EntityTypeBuilder builder) where TEntity : class, ISoftDelete
    {
        builder.HasQueryFilter((TEntity d) => d.IsDeleted!=true);
    }
    public static void ConfigureSoftDeleteV2<TEntity>(EntityTypeBuilder builder) where TEntity : class, ISoftDeleteV2
    {
        builder.HasQueryFilter((TEntity d) => d.IsDeleted!=true);
    }
}