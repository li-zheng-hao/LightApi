namespace LightApi.Core.Options;

/// <summary>
/// 秘钥相关配置
/// </summary>
public class SecretOptions
{
    /// <summary>
    /// section名称
    /// </summary>
    public const string SectionName = "Secret";

    /// <summary>
    /// 对称加密秘钥
    /// </summary>
    public string? SecretKey { get; set; }
    
    /// <summary>
    /// 非对称加密公钥
    /// </summary>
    public string? RsaPublicKey { get; set; }
    
    /// <summary>
    /// 非对称加密私钥
    /// </summary>
    public string? RsaPrivateKey { get; set; }
}