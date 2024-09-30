using System;
using System.Reflection;

namespace LightApi.Mongo;

public static class EntityCache
{
    static Dictionary<Type, PropertyInfo> IdNameCache = new();
    /// <summary>
    /// 获取实体的Id值
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static object GetIdValue(object entity)
    {
        var type = entity.GetType();
        if (IdNameCache.TryGetValue(type, out var propertyInfo))
        {
            return propertyInfo.GetValue(entity)!;
        }
        propertyInfo = type.GetProperty("Id");
        if (propertyInfo == null)
        {
            throw new InvalidOperationException($"实体{type.Name}没有Id属性");
        }
        IdNameCache.Add(type, propertyInfo!);
        return propertyInfo!.GetValue(entity)!;
    }
}
