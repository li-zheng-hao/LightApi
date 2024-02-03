namespace LightApi.Service.Dtos.User;

public class UserInfoDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }
    
    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; }
    
    /// <summary>
    /// 头像地址
    /// </summary>
    public string Avatar { get; set; }
    /// <summary>
    /// 角色
    /// </summary>
    public List<string> Roles { get; set; } = new();
    

    /// <summary>
    /// 权限
    /// </summary>
    public List<string> UserPermissions { get; set; } = new();
}