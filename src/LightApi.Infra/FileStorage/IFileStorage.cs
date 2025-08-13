using Microsoft.AspNetCore.Http;
using Minio.Exceptions;

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
    Task<string> UploadToLocalStorage(Stream stream, string fileName);

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
    /// <exception cref="InvalidOperationException">当Minio client未初始化时抛出异常</exception>
    /// <exception cref="MinioException">当上传失败时抛出异常</exception>
    /// <returns>返回文件的完整url</returns>
    Task<string> UploadToMinioStorage(IFormFile file);

    /// <summary>
    /// 上传到Minio存储
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <exception cref="InvalidOperationException">当Minio client未初始化时抛出异常</exception>
    /// <exception cref="MinioException">当上传失败时抛出异常</exception>
    /// <returns>返回文件的完整url</returns>
    Task<string> UploadToMinioStorage(Stream stream, string fileName);

    /// <summary>
    /// 上传到MongoDB
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="isTempFile">是否为临时文件（用于定期删除）</param>
    /// <returns>PublicDomain/文件id_文件名(带后缀)</returns>
    Task<string?> UploadToMongoDBStorage(Stream stream, string fileName, bool isTempFile = false);

    /// <summary>
    /// 从mongodb下载文件
    /// </summary>
    /// <param name="key">UploadToMongoDB接口返回的字符串，或者直接使用文件id,或者文件url</param>
    /// <returns>文件流</returns>
    Task<Stream?> DownloadFromMongoDBStorage(string key);

    /// <summary>
    /// 从mongodb删除文件
    /// </summary>
    /// <param name="key">UploadToMongoDB接口返回的字符串，或者直接使用文件id,或者文件url</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteFromMongoDBStorage(string key);

    /// <summary>
    /// 生成Minio预签名的上传链接
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>完整的下载签名url</returns>
    Task<string> GenerateMinioUploadUrl(string fileName);

    /// <summary>
    /// 生成Minio分享下载的链接
    /// </summary>
    /// <param name="objectKey"></param>
    /// <param name="expireSeconds">过期时间</param>
    /// <returns></returns>
    Task<string> GenerateMinioDownloadUrl(string objectKey, int expireSeconds = 1800);

    /// <summary>
    /// 从当前FileStorage minio生成的url获取objectKey
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    string GetMinioObjectKeyFromUrl(string url);
}
