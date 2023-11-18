using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace LightApi.Core.Helper;

public static class StringHelper
{
    /// <summary>
    /// 判断是否有中文字符
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool HasChineseChar(string target)
    {
        if (string.IsNullOrWhiteSpace(target)) return false;
        
        return Regex.IsMatch(target, @"[\u4e00-\u9fa5]");
    }
    
    
    /// <summary>
    /// 过滤中文字符
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string FilterChineseChar(string target)
    {
        if (string.IsNullOrWhiteSpace(target)) return target;
          
        return Regex.Replace(target, @"[\u4e00-\u9fa5]", "");
    }
    /// <summary>
    /// 判断是否为Url地址
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsUrl(string target)
    {
        if (string.IsNullOrWhiteSpace(target)) return false;
        // Try to create a Uri instance
        if (Uri.TryCreate(target, UriKind.Absolute, out var uriResult))
        {
            // Check if the Uri has a valid scheme and host
            return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
        }
    
        return false;
    }
    
    /// <summary>
    /// 判断是否为Json字符串
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsJson(string target)
    {
        if (string.IsNullOrWhiteSpace(target)) return false;
        try
        {
            var obj = JToken.Parse(target);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    
        return false;
    }
}