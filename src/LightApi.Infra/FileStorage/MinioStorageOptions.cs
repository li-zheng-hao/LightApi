namespace LightApi.Infra.FileStorage;

public class MinioStorageOptions
{
    public string EndPoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Bucket { get; set; }

    /// <summary>
    /// 是否使用https
    /// </summary>
    public bool EnableSSL { get; set; } = false;
}
