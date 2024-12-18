using Microsoft.Extensions.Options;

namespace LightApi.Infra.FileStorage;

public class StorageOptions
{
    /// <summary>
    /// 本地文件存储配置
    /// </summary>
    public LocalStorageOptions? LocalStorageOptions { get; set; }

    /// <summary>
    /// MinIO文件存储配置
    /// </summary>
    public MinioStorageOptions? MinioStorageOptions { get; set; }

    /// <summary>
    /// MongoDb文件存储配置
    /// </summary>
    public MongoStorageOptions? MongoStorageOptions { get; set; }
}
