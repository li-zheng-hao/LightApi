namespace LightApi.Infra.FileStorage;

public class MongoStorageOptions
{
    /// <summary>
    /// 件前缀拼接域名 如果自己写了一个接口 http://xxx.com/api/file 则这里需要填写该前缀
    /// </summary>
    public string PublicDomain { get; set; }
}
