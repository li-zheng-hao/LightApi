namespace LightApi.Infra.Extension.DynamicQuery;

/// <summary>
/// 动态操作类型
/// </summary>
public enum DynamicOpType
{
    /// <summary>
    /// 等于
    /// </summary>
    Equal,

    /// <summary>
    /// 不等于
    /// </summary>
    NotEqual,

    /// <summary>
    /// 大于
    /// </summary>
    GreaterThan,

    /// <summary>
    /// 大于或等于
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// 小于
    /// </summary>
    LessThan,

    /// <summary>
    /// 小于获等于
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// 包含
    /// </summary>
    Contains,

    /// <summary>
    /// 为空
    /// </summary>
    IsNull,

    /// <summary>
    /// 不为空
    /// </summary>
    IsNotNull,

    /// <summary>
    /// 参数包含目标
    /// </summary>
    In,

    /// <summary>
    /// 开头包含
    /// </summary>
    StartWith
}
