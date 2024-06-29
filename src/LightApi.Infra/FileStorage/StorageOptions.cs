using Microsoft.Extensions.Options;

namespace LightApi.Infra.FileStorage;

public class StorageOptions
{
    /// <summary>
    /// 本地文件存储配置
    /// </summary>
    public LocalStorageOptions? LocalStorageOptions { get; set; }
    
    /// <summary>
    /// MinIO文件读取配置
    /// </summary>
    public MinioStorageOptions? MinioStorageOptions { get; set; }
}

