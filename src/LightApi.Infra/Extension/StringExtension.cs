using System.Text;
using System.Text.RegularExpressions;

namespace LightApi.Infra.Extension;

public static class StringExtension
{
    public static bool IsNullOrWhiteSpace(this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static bool IsNotNullOrWhiteSpace(this string? str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// 判断字符串长度是否为24
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool Is24Length(this string str)
    {
        return str?.Length == 24;
    }

    /// <summary>
    /// 过滤中文
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string FilterChineseChar(this string source)
    {
        Regex p_regex = new Regex("^[\u4e00-\u9fa5]{0,}$");
        StringBuilder builder = new();

        foreach (var t in source)
        {
            if (p_regex.IsMatch(t.ToString()) == false)
            {
                builder.Append(t);
            }
        }

        return builder.ToString();
    }
    /// <summary>
    /// 是否包含中文
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool HasChineseChar(this string source)
    {
        if (string.IsNullOrWhiteSpace(source)) return false;
        
        return Regex.IsMatch(source, @"[\u4e00-\u9fa5]");  
    }
    /// <summary>
    /// 填充字符串
    /// </summary>
    /// <param name="source"></param>
    /// <param name="num"></param>
    /// <param name="character"></param>
    /// <returns></returns>
    public static string ZFill(this string source, int num, char character = '0')
    {
        return source.PadLeft(num, character);
    }
    /// <summary>
    /// 忽略大小写比较
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool EqualIgnoreCase(this string source, string target)
    {
        return source.Equals(target, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// 忽略大小写比较
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool StartWithIgnoreCase(this string source, string target)
    {
        return source.StartsWith(target, StringComparison.OrdinalIgnoreCase);
    }
}