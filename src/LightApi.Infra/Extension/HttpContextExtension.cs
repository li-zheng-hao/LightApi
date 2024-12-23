﻿using Masuit.Tools;
using Microsoft.AspNetCore.Http;

namespace LightApi.Infra.Extension;

public static class HttpContextExtension
{
    /// <summary>
    /// 从HttpContext.Items中获取值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetItem<T>(this HttpContext context, string key)
    {
        context.Items.TryGetValue(key, out var value);
        return value switch
        {
            null => default,
            T t => t,
            _ => throw new ArgumentException("类型不匹配")
        };
    }
}
