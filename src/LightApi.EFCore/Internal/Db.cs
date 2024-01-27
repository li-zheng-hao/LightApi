using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.Entities;

namespace LightApi.EFCore.Internal;

/// <summary>
/// LightApi.EfCore内部全局配置
/// </summary>
public static class Db
{
    /// <summary>
    /// 数据库上下文和IEntityInfo类型的映射关系，用于在多库环境且不同数据库不同表的情况下配置不同DbContext拥有的模型
    /// </summary>
    private static Dictionary<Type, Type> DbContextModelMap { get; set; } = new();
    /// <summary>
    /// 数据库上下文事务单元类型映射关系表
    /// </summary>
    private static Dictionary<Type, Type> DbContextUnitOfWorkMap { get; set; } = new();

    /// <summary>
    /// 增加新的配置
    /// </summary>
    /// <param name="dbContextType"></param>
    /// <param name="iEntityInfoType"></param>
    public static void AddDbContextModelMap(Type dbContextType, Type iEntityInfoType)
    {
        if (DbContextModelMap.ContainsKey(dbContextType)) return;
        DbContextModelMap.Add(dbContextType,iEntityInfoType);
    }

    /// <summary>
    /// 获取AppDbContext对应的模型配置
    /// </summary>
    /// <param name="appDbContext"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static Type GetEntityInfoType(AppDbContext appDbContext)
    {
        var contextType=appDbContext.GetType();
        if (!DbContextModelMap.ContainsKey(contextType)) throw new KeyNotFoundException($"没有找到对应{contextType}的IEntityInfo配置");
        return DbContextModelMap[contextType];
    }
    
    /// <summary>
    /// 新增上下文和工作单元映射关系
    /// </summary>
    /// <param name="dbContextType"></param>
    /// <param name="unitofWorkType"></param>
    public static void AddDbContextUnitOfWorkMap(Type dbContextType, Type unitofWorkType)
    {
        if (DbContextUnitOfWorkMap.ContainsKey(dbContextType)) return;
        DbContextUnitOfWorkMap.Add(dbContextType,unitofWorkType);
    }
  
    /// <summary>
    /// 获取上下文和工作单元映射关系
    /// </summary>
    /// <param name="contextType"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static Type GetDbContextUnitOfWorkMap(Type contextType)
    {
        if (!DbContextUnitOfWorkMap.ContainsKey(contextType)) throw new KeyNotFoundException($"没有找到对应{contextType}的工作单元类型");
        return DbContextUnitOfWorkMap[contextType];
    }
}