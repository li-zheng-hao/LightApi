using System.Collections;

namespace LightApi.Infra.Extension;

public static class SafeExtension
{

    /// <summary>
    /// 安全地截断字符串
    /// </summary>
    /// <param name="source"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string SafeSubString(this string source, uint length)
    {
        if (source.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        if (source.Length > length)
        {
            return source.Substring(0, (int) length);
        }

        return source;
    }
    /// <summary>
    /// List列表安全ToString
    /// </summary>
    /// <param name="target"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string SafeToString<T>(this IEnumerable<T> target, string splitChat)
    {
        Check.ThrowIf(!splitChat.IsNotNullOrWhiteSpace(), new ArgumentNullException("splitChat不能为空"));
        
        if (target != null && target.Any())
        {
            return string.Join(splitChat, target.Select(it => it.ToString()));
        }

        return string.Empty;
    }

    /// <summary>
    /// 安全ToString
    /// </summary>
    /// <param name="target"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string SafeToString<T>(this T target)
    {
        return target is null ? string.Empty : target.ToString();
    }

    /// <summary>
    /// 安全的调用对象
    /// </summary>
    /// <param name="target"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void SafeCall<T>(this T target, Action<T> action)
    {
        if (target is ICollection { Count: > 0 })
        {
            action(target);

            return;
        }

        if (target != null)
            action(target);
    }

    /// <summary>
    /// 安全的调用对象
    /// </summary>
    /// <param name="target"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static T2 SafeCall<T1, T2>(this T1 target, Func<T1, T2> action)
    {
        if (target is ICollection { Count: > 0 })
        {
            return action(target);
        }

        return target != null ? action(target) : default;
    }
    
    /// <summary>
    /// 安全的调用对象 用于处理async方法
    /// </summary>
    /// <param name="target"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static async Task<T2> SafeCall<T1,T2>(this T1 target,Func<T1,Task<T2>> action)
    {
        if (target is ICollection { Count: > 0 })
        {
            return await action(target);
        }
        return target != null ? await action(target) : default;
    }
}