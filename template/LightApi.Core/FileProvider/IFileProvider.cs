using Microsoft.AspNetCore.Http;

namespace LightApi.Core.FileProvider;

public interface IFileProvider
{
    /// <summary>
    /// 根路径 不设置的情况下默认存储当前程序目录下的FileData文件夹
    /// </summary>
    string RootDir { get; set; }

    /// <summary>
    /// 命名规则 默认md5
    /// </summary>
    FileNameGenerateStrategy FileNameGenerateStrategy { get; set; }
    
    Task<(string? fileUrl, string? md5)> SaveFile(Stream stream, string fileName);


    Task<(string? fileUrl, string? md5)> SaveFile(IFormFile file);
    
    Task<(string? fileUrl, string? md5)> SaveFile(byte[] fileBytes, string fileName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileUrl"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    Task<Stream?> GetStream(string fileUrl);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileUrl"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    Task<byte[]?> GetFileBytes(string fileUrl);

    /// <summary>
    /// 删除文件 不存在则忽略
    /// </summary>
    /// <param name="fileUrl"></param>
    Task DeleteFile(string fileUrl);
}

public enum FileNameGenerateStrategy
{
    /// <summary>
    /// 按日期划分文件夹
    /// </summary>
    TimeStampWithDayPrefix,
    /// <summary>
    /// 所有文件在同一个目录下，文件名为md5
    /// </summary>
    MD5
}