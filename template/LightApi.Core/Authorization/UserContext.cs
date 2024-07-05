using LightApi.Infra.Http;

namespace LightApi.Core.Authorization;

/// <summary>
/// 登录用户上下文 Scoped级别
/// </summary>
public class UserContext:IUser
{
    public string Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

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
        return !string.IsNullOrWhiteSpace(Id);
    }
    
}