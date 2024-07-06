using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using LightApi.Infra.InfraException;
using Newtonsoft.Json.Linq;

namespace LightApi.Infra.Extension.DynamicQuery;

public static class DyanmicQueryExtension
{
    /// <summary>
    /// 动态查询条件
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <param name="opType"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="BusinessException"></exception>
    /// <exception cref="Exception"></exception>
    public static IQueryable<T> DynamicWhere<T>(this IQueryable<T> queryable, string property, object? value,
        DynamicOpType opType)
    {
        PropertyInfo? propertyInfo = null;
        MemberExpression? propertyAccess = null;
        var parameter = Expression.Parameter(typeof(T), "x");
        if (property.Contains("."))
        {
            // 支持多级属性，如 "User.Name"
            var properties = property.Split('.');
            propertyAccess = Expression.PropertyOrField(parameter, properties[0]);
            for (int i = 1; i < properties.Length; i++)
            {
                propertyAccess = Expression.PropertyOrField(propertyAccess, properties[i]);
                propertyInfo = propertyAccess.Member as PropertyInfo;
            }
        }
        else
        {
            propertyInfo = typeof(T).GetProperty(property);
            if (propertyInfo == null) throw new BusinessException("未找到属性" + property);
            propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        }

        Expression? comparison = null;
        switch (opType)
        {
            case DynamicOpType.Contains:
                Check.NotNullOrEmpty(value,"Contains操作符的值不能为空");
                var targetValue = ConvertType(value, propertyInfo!.PropertyType);
                var constant = Expression.Constant(targetValue, propertyInfo.PropertyType);
                var methodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                // 使用 Expression.Condition 创建条件表达式
                comparison = Expression.Condition(
                    Expression.NotEqual(propertyAccess, Expression.Constant(null, propertyInfo.PropertyType)), // 条件：propertyAccess 不等于 null
                    Expression.Call(propertyAccess, methodInfo!, constant), // 如果条件成立，则调用 Contains 方法
                    Expression.Constant(false) // 如果条件不成立，则返回 false
                );
                break;
            case DynamicOpType.Equal:
                Check.ThrowIf(value==null&&!IsNullableType(propertyInfo!.PropertyType)
                    ,"目标类型不允许为null");
                targetValue = ConvertType(value, propertyInfo!.PropertyType);
                constant = Expression.Constant(targetValue,propertyInfo.PropertyType);
                comparison = Expression.Equal(propertyAccess, constant);
                break;
            case DynamicOpType.NotEqual:
                Check.ThrowIf(value==null&&!IsNullableType(propertyInfo!.PropertyType)
                    ,"目标类型不允许为null");
                targetValue = ConvertType(value, propertyInfo!.PropertyType);
                constant = Expression.Constant(targetValue,propertyInfo.PropertyType);
                comparison = Expression.NotEqual(propertyAccess, constant);
                break;
            case DynamicOpType.GreaterThan:
                Check.NotNullOrEmpty(value,"GreaterThan操作符的值不能为空");
                targetValue = ConvertType(value, propertyInfo!.PropertyType);
                constant = Expression.Constant(targetValue,propertyInfo.PropertyType);
                comparison = Expression.GreaterThan(propertyAccess, constant);
                break;
            case DynamicOpType.GreaterThanOrEqual:
                Check.NotNullOrEmpty(value,"GreaterThanOrEqual操作符的值不能为空");
                targetValue = ConvertType(value, propertyInfo!.PropertyType);
                constant = Expression.Constant(targetValue,propertyInfo.PropertyType);
                comparison = Expression.GreaterThanOrEqual(propertyAccess, constant);
                break;
            case DynamicOpType.LessThan:
                Check.NotNullOrEmpty(value,"LessThan操作符的值不能为空");
                targetValue = ConvertType(value, propertyInfo!.PropertyType);
                constant = Expression.Constant(targetValue,propertyInfo.PropertyType);
                comparison = Expression.LessThan(propertyAccess, constant);
                break;
            case DynamicOpType.LessThanOrEqual:
                Check.NotNullOrEmpty(value,"LessThanOrEqual操作符的值不能为空");
                targetValue = ConvertType(value, propertyInfo!.PropertyType);
                constant = Expression.Constant(targetValue,propertyInfo.PropertyType);
                comparison = Expression.LessThanOrEqual(propertyAccess, constant);
                break;
            case DynamicOpType.IsNull:
                comparison = Expression.Equal(propertyAccess, Expression.Constant(null,propertyInfo!.PropertyType));
                break;
            case DynamicOpType.IsNotNull:
                comparison = Expression.NotEqual(propertyAccess, Expression.Constant(null,propertyInfo!.PropertyType));
                break;
            case DynamicOpType.StartWith:
                Check.NotNullOrEmpty(value,"StartWith操作符的值不能为空");
                targetValue = ConvertType(value, propertyInfo!.PropertyType);
                constant = Expression.Constant(targetValue,propertyInfo.PropertyType);
                MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) })!;
                // 使用 Expression.Condition 创建条件表达式
                comparison = Expression.Condition(
                    Expression.NotEqual(propertyAccess, Expression.Constant(null, propertyInfo.PropertyType)), // 条件：propertyAccess 不等于 null
                    Expression.Call(propertyAccess, mi!, constant), // 如果条件成立，则调用 Contains 方法
                    Expression.Constant(false) // 如果条件不成立，则返回 false
                );
                break;
            case DynamicOpType.In:
                var genericListType = typeof(List<>).MakeGenericType(propertyInfo!.PropertyType);
                var genericList = Activator.CreateInstance(genericListType);
                IEnumerable? list ;
                if (value is JsonElement jsonElement)
                    list = jsonElement.Deserialize<IEnumerable>();
                else 
                    list =  value as IEnumerable;
                if (list == null) throw new BusinessException("value类型错误");

                foreach (var item in list)
                    genericListType.GetMethod("Add")!.Invoke(genericList,
                        new[] { ConvertType(item, propertyInfo.PropertyType) });

                MethodInfo containsMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(mi => mi.Name == "Contains" && mi.GetParameters().Length == 2)
                    .MakeGenericMethod(propertyInfo!.PropertyType);

                // 如果value的类型与property的类型不同，则可能需要进行转换  
                Expression convertedValue = Expression.Constant(genericList,
                    typeof(IEnumerable<>).MakeGenericType(propertyInfo.PropertyType));
                Expression propertyValue = Expression.Convert(propertyAccess, propertyInfo.PropertyType);

                comparison = Expression.Call(containsMethod, convertedValue, propertyValue);
                break;
            default:
                throw new BusinessException("操作不支持");
        }

        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        queryable = queryable.Where(lambda);

        return queryable;
    }

    private static object? ConvertType(object? value, Type type)
    {
        try
        {
            if (value == null) 
                return null;
            if (value.GetType() == type)
                return value;
            if (value is JValue jValue)
            {
                var res=jValue.ToObject(type);
                return res;
            }else if (value is System.Text.Json.JsonElement jsonElement)
            {
                var targetType = Nullable.GetUnderlyingType(type) ?? type; // avoid type becoming null

                var serializerOptions = new JsonSerializerOptions()
                {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString ,
                    Converters = { new JsonStringEnumConverter() }
                };
                var res = jsonElement.Deserialize(targetType,serializerOptions);
                return res;
            }
            else
            {
                var targetType = Nullable.GetUnderlyingType(type) ?? type; // avoid type becoming null
                var conv = TypeDescriptor.GetConverter(targetType);
                
                if(conv.CanConvertFrom(value.GetType()))
                {
                    return conv.ConvertFrom(value);
                }
                return Convert.ChangeType(value,targetType);
            }
        }
        catch(Exception e)
        {
            Debug.WriteLine($"类型转换错误:{e.Message}");
            throw new BusinessException($"查询参数和类型不一致,目标类型{type.FullName},参数类型{value.GetType().FullName}");
        }
    }
    /// <summary>
    /// 是否为nullable类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsNullableType(Type type)
    {
        // class类型为引用类型，永远都可以为null
        if(type.IsValueType==false) return true;
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}