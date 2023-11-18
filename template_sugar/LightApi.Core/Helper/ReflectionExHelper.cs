using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LightApi.Core.Helper;

public class ReflectionExHelper
{
    /// <summary>
    /// 获取DisplayAttribute特性的Name
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static string? GetDisplayName(Type type, string fieldName)
    {
        var field = type.GetField(fieldName);

        if (field == null)
        {
            throw new KeyNotFoundException($"{fieldName} 在{type.Name}中不存在");
        }

        // 获取 Display 特性
        var displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute))!;

        return displayAttribute?.Name;
    }
    
    /// <summary>
    /// 获取DisplayAttribute特性的Name
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static string? GetDescriptionName(Type type, string fieldName)
    {
        var field = type.GetField(fieldName);

        if (field == null)
        {
            throw new KeyNotFoundException($"{fieldName} 在{type.Name}中不存在");
        }

        // 获取 Display 特性
        var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))!;

        return descriptionAttribute?.Description;
    }

    /// <summary>
    /// 获取类的字段名
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static string GetFieldName(Type type, string fieldName)
    {
        var field = type.GetProperties().FirstOrDefault(it=>it.Name.ToLower()==fieldName.ToLower());

        if (field == null)
        {
            throw new KeyNotFoundException($"{fieldName} 在{type.Name}中不存在");
        }

        return field.Name;
    }
}