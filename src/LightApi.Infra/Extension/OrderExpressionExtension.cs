﻿using System.Linq.Expressions;
using System.Reflection;
using LightApi.Infra.InfraException;

namespace LightApi.Infra.Extension;

/// <summary>
/// 动态排序
/// </summary>
public static class OrderExpressionExtension
{
    /// <summary>
    /// 根据字段名动态排序
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fieldName">字段名 支持多级 如Property1.SubProperty1</param>
    /// <param name="isDescending">是否降序 不传默认false</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="KeyNotFoundException">当属性不存在时会抛出此异常</exception>
    /// <returns></returns>
    public static IOrderedQueryable<T> DynamicOrder<T>(this IQueryable<T> source, string fieldName, bool? isDescending)
    {
        return isDescending == true ? source.OrderByDescending(fieldName) : source.OrderBy(fieldName);
    }

    #region 私有方法

    private static IOrderedQueryable<T> OrderBy<T>(
        this IQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "OrderBy");
    }

    private static IOrderedQueryable<T> OrderByDescending<T>(
        this IQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "OrderByDescending");
    }

    private static IOrderedQueryable<T> ThenBy<T>(
        this IOrderedQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "ThenBy");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="property"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static IOrderedQueryable<T> ThenByDescending<T>(
        this IOrderedQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "ThenByDescending");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="property"></param>
    /// <param name="methodName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    private static IOrderedQueryable<T> ApplyOrder<T>(
        IQueryable<T> source,
        string property,
        string methodName)
    {
        string[] props = property.Split('.');
        Type type = typeof(T);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;
        foreach (string prop in props)
        {
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo? pi = type.GetProperty(prop);
            if (pi == null)
                throw new BusinessException($"排序字段不存在");
            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }

        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

        // 必须有对应方法
        object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), type)
            .Invoke(null, new object[] { source, lambda })!;

        return (IOrderedQueryable<T>)result;
    }

    #endregion
}