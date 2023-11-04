namespace LightApi.Core.Authorization;

/// <summary>
/// 登录用户上下文 Scoped级别
/// </summary>
public class UserContext
{
    public int Id { get; set; } = -1;

    public string? Name { get; set; }

    /// <summary>
    /// 用户角色，通过","分割
    /// </summary>
    public string? Roles { get; set; }
    
    /// <summary>
    /// 用户权限
    /// </summary>
    public string? Permissions { get; set; }

    public bool IsAuthenticated()
    {
        return Id != -1;
    }
    
}