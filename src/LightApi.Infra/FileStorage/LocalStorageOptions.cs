using Microsoft.Extensions.Options;

namespace LightApi.Infra.FileStorage;

public class LocalStorageOptions
{
    /// <summary>
    /// 本地文件夹 可以是绝对路径，也可以是相对路径(本地文件)
    /// </summary>
    public string Directory { get; set; } = "Storage";

    /// <summary>
    /// 文件前缀拼接域名 如果自己写了一个接口 http://xxx.com/api/file 则这里需要填写该前缀
    /// </summary>
    public string PublicDomain { get; set; }
}
