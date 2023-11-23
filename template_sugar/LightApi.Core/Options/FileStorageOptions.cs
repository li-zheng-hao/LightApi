using LightApi.Core.FileProvider;

namespace LightApi.Core.Options;

public class FileStorageOptions
{
    /// <summary>
    /// section名称
    /// </summary>
    public const string SectionName = "FileStorage";

    /// <summary>
    /// 本地存储，支持多目录
    /// </summary>
    public List<LocalStorage>? LocalStorages { get; set; }
    
    /// <summary>
    /// MinIO存储配置
    /// </summary>
    public List<MinIOStorage>? MinIOStorages { get; set; }
}

/// <summary>
/// 本地存储
/// </summary>
public class LocalStorage
{
    /// <summary>
    /// 存储名称
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// 存储根目录 绝对路径或相对路径
    /// </summary>
    public string StorageRootDir { get; set; }
    
    /// <summary>
    /// 文件命名策略
    /// </summary>
    public FileNameGenerateStrategy FileNameGenerateStrategy { get; set; }
}

public class MinIOStorage
{
    /// <summary>
    /// 存储名称
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// 存储根目录 绝对路径或相对路径
    /// </summary>
    public string StorageRootDir { get; set; }
    
    /// <summary>
    /// 文件命名策略
    /// </summary>
    public FileNameGenerateStrategy FileNameGenerateStrategy { get; set; }
    
    /// <summary>
    /// 桶名称
    /// </summary>
    public string Bucket { get; set; }
    /// <summary>
    /// 域名
    /// </summary>
    public string Endpoint { get; set; }
    /// <summary>
    /// 访问Key
    /// </summary>
    public string AccessKey { get; set; }
    /// <summary>
    /// 秘钥
    /// </summary>
    public string SecretKey { get; set; }
    
    /// <summary>
    /// 公网域名 给文件地址用的
    /// </summary>
    public string PublicDomain { get; set; }
    
}