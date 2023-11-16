using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightApi.EFCore.Config;

/// <summary>
/// 定义实体类型配置基类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class AbstractEntityTypeConfiguration<TEntity,TKey> : IEntityTypeConfiguration<TEntity>
   where TEntity : class,IEfEntity<TKey> 
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // var entityType = typeof(TEntity);
        // ConfigureKey(builder, entityType);
        // ConfigureConcurrency(builder, entityType);
        // ConfigureQueryFilter(builder, entityType);
    }

    // protected virtual void ConfigureKey(EntityTypeBuilder<TEntity> builder, Type entityType)
    // {
    //     builder.HasKey(x => x.Id);
    //
    //     if (entityType.HasAttribute<NonAutoIncrementAttribute>())
    //     {
    //         builder.Property(x => x.Id).ValueGeneratedNever();
    //     }
    // }

    // protected virtual void ConfigureConcurrency(EntityTypeBuilder<TEntity> builder, Type entityType)
    // {
    //     // if (typeof(IConcurrency).IsAssignableFrom(entityType))
    //         // builder.Property("RowVersion").IsRequired().IsRowVersion().ValueGeneratedOnAddOrUpdate();
    // }

    // protected virtual void ConfigureQueryFilter(EntityTypeBuilder<TEntity> builder, Type entityType)
    // {
    //     if (typeof(ISoftDelete).IsAssignableFrom(entityType))
    //     {
    //         builder.HasQueryFilter(d => !EF.Property<bool>(d, "IsDeleted"));
    //     }
    //     else if (typeof(ISoftDeleteV2).IsAssignableFrom(entityType))
    //     {
    //         builder.HasQueryFilter(d => EF.Property<bool>(d, "IsDeleted")!=true);
    //     }
    // }
}