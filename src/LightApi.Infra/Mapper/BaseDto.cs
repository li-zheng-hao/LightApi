using Mapster;

namespace LightApi.Infra.Mapper;

/// <summary>
/// https://medium.com/@M-S-2/enjoy-using-mapster-in-net-6-2d3f287a0989
/// </summary>
/// <typeparam name="TDto"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public abstract class BaseDto<TDto, TEntity> : IRegister
    where TDto : class, new()
    where TEntity : class, new()
{

    public TEntity ToEntity()
    {
        return this.Adapt<TEntity>();
    }

    public TEntity ToEntity(TEntity entity)
    {
        return (this as TDto).Adapt(entity);
    }

    public static TDto FromEntity(TEntity entity)
    {
        return entity.Adapt<TDto>();
    }


    private TypeAdapterConfig Config { get; set; }

    public virtual void AddCustomMappings() { }

    /// <summary>
    /// DTO到Entity的映射
    /// </summary>
    /// <returns></returns>
    protected TypeAdapterSetter<TDto, TEntity> SetCustomMappings()
        => Config.ForType<TDto, TEntity>();
    
    
    /// <summary>
    /// T1到T2的映射
    /// </summary>
    /// <returns></returns>
    protected TypeAdapterSetter<T1, T2> SetCustomMappings<T1,T2>()
        => Config.ForType<T1, T2>();
    
    
    /// <summary>
    /// T2到T1的映射
    /// </summary>
    /// <returns></returns>
    protected TypeAdapterSetter<T2, T1> SetCustomMappingsReverse<T1,T2>()
        => Config.ForType<T2, T1>();
    
    
    /// <summary>
    /// Entity到Dto的映射
    /// </summary>
    /// <returns></returns>
    protected TypeAdapterSetter<TEntity, TDto> SetCustomMappingsInverse()
        => Config.ForType<TEntity, TDto>();

    public void Register(TypeAdapterConfig config)
    {
        Config = config;
        AddCustomMappings();
    }
}