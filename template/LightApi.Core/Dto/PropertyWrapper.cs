namespace LightApi.Core.Dto;

/// <summary>
/// 用来做局部更新
/// </summary>
/// <typeparam name="T"></typeparam>
public class PropertyWrapper<T>
{
    public T Value { get; set; }
}