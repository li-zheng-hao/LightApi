using System.Reflection;

namespace LightApi.Infra.Helper;

public static class ReflectionHelper
{
    public static bool HasAttribute(this MethodInfo method, Type attributeType)
    {
        return method.GetCustomAttributes(attributeType, true).Length > 0;
    }

    public static MethodInfo[] GetAllMethods(this Type type)
    {
        return type.GetMethods();
    }

    public static MethodInfo[] GetMethodsByAttribute(this Type type, Type attributeType)
    {
        return type.GetMethods().Where(m => m.HasAttribute(attributeType)).ToArray();
    }

    public static bool HasAttribute(this Type type, Type attributeType)
    {
        return type.GetMethods().Any(m => m.HasAttribute(attributeType));
    }

    /// <summary>
    /// 创建类实例
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object CreateInstance(Type type, params object[] args)
    {
        return Activator.CreateInstance(type, args)!;
    }

    /// <summary>
    /// Call Function
    /// </summary>
    /// <param name="target"></param>
    /// <param name="functionName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object? CallFunction(
        this object target,
        string functionName,
        params object[] args
    )
    {
        var method = target.GetType().GetMethod(functionName);
        return method?.Invoke(target, args);
    }

    /// <summary>
    /// Get Property Value
    /// </summary>
    /// <param name="target"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static object? GetPropertyValue(this object target, string propertyName)
    {
        var property = target.GetType().GetProperty(propertyName);
        return property?.GetValue(target);
    }
}
