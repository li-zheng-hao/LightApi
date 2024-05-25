using System.Collections;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using LightApi.Infra.InfraException;

namespace LightApi.Infra.Extension;

public static class Check
{
    /// <summary>
    /// 检查对象是否为null, 字符串的话检查是否为空字符串, 集合的话检查是否为空集合，是的话抛出业务异常
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="ex"></param>
    public static void NotNullOrEmpty(object? obj, System.Exception ex)
    {
        if (obj is null)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }

        if (obj is string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        if (obj is IEnumerable enumerable)
        {
            if (!enumerable.GetEnumerator().MoveNext())
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }
    }

    /// <summary>
    /// 检查对象是否为null, 字符串的话检查是否为空字符串, 集合的话检查是否为空集合，是的话抛出业务异常
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="errorMessage"></param>
    /// <exception cref="BusinessException"></exception>
    public static void NotNullOrEmpty(object? obj, string errorMessage)
    {
        if (obj is null)
        {
            throw new BusinessException(errorMessage);
        }

        if (obj is string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new BusinessException(errorMessage);
            }
        }

        if (obj is IEnumerable enumerable)
        {
            if (!enumerable.GetEnumerator().MoveNext())
            {
                throw new BusinessException(errorMessage);
            }
        }
    }

    /// <summary>
    /// 检查表达式是否为真,真的话记录错误消息并返回false
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="errMessage"></param>
    /// <returns></returns>
    public static bool CheckIf(bool expression, string errMessage)
    {
        return !expression;
    }

    /// <summary>
    /// 检查表达式是否为真,真的话记录错误消息并抛出异常
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="errMessage"></param>
    /// <exception cref="BusinessException"></exception>
    public static void ThrowIf(bool expression, string errMessage)
    {
        if (expression)
        {
            throw new BusinessException(errMessage);
        }
    }

    /// <summary>
    /// 检查表达式是否为真,真的话记录错误消息并抛出异常
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="ex"></param>
    public static void ThrowIf(bool expression, Exception ex)
    {
        if (expression)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }
    /// <summary>
    /// 空的话抛出业务异常
    /// </summary>
    /// <param name="source"></param>
    /// <param name="errorMessage"></param>
    public static void NotNullOrEmptyEx(this object? source,string errorMessage="数据不存在")
    {
        NotNullOrEmpty(source,errorMessage);
    }

    /// <summary>
    /// 条件为真时抛出业务异常
    /// </summary>
    /// <param name="source"></param>
    /// <param name="errMessage"></param>
    /// <exception cref="BusinessException"></exception>
    public static void ThrowIfEx(this bool source, string errMessage)
    {
        if (source)
            throw new BusinessException(errMessage);
    }
}