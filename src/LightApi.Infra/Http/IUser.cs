namespace LightApi.Infra.Http;

/// <summary>
/// 用户
/// </summary>
public interface IUser
{
    /// <summary>
    /// ID
    /// </summary>
    int Id { get; set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    string UserName { get; set; }
}