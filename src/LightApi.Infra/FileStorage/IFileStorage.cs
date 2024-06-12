using Microsoft.AspNetCore.Http;

namespace LightApi.Infra.FileStorage;

public interface IFileStorage
{
    /// <summary>
    /// 上传到本地存储
    /// </summary>
    /// <param name="file"></param>
    /// <returns>返回文件 http url</returns>
    Task<string> UploadToLocalStorage(IFormFile file);

    /// <summary>
    /// 上传到本地存储
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <returns>返回文件 http url</returns>
    Task<string> UploadToLocalStorage(Stream stream,string fileName);

    /// <summary>
    /// 从本地存储下载
    /// </summary>
    /// <param name="subPath">文件相对路径</param>
    /// <returns>返回文件http url 文件不存在则返回null</returns>
    FileStream? DownloadFromLocalStorage(string subPath);
  
    /// <summary>
    /// 上传到Minio存储
    /// </summary>
    /// <param name="file"></param>
    /// <returns>返回文件 http url,失败返回null</returns>
    Task<string?> UploadToMinioStorage(IFormFile file);


    /// <summary>
    /// 上传到本地存储
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <returns>返回文件 http url</returns>
    Task<string?> UploadToMinioStorage(Stream stream,string fileName);
}