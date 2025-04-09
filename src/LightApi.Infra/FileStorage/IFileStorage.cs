﻿using Microsoft.AspNetCore.Http;

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
    /// <returns>返回文件 http url,失败返回null</returns>
    Task<string?> UploadToMinioStorage(IFormFile file);

    /// <summary>
    /// 上传到本地存储
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <returns>返回文件 http url</returns>
    Task<string?> UploadToMinioStorage(Stream stream, string fileName);

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
    /// <param name="key">UploadToMongoDB接口返回的字符串，或者直接使用文件id</param>
    /// <returns></returns>
    Task<Stream?> DownloadFromMongoDBStorage(string key);

    /// <summary>
    /// 生成Minio预签名的上传链接
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>完整的下载签名url</returns>
    Task<string> GenerateMinioUploadUrl(string fileName);
}
