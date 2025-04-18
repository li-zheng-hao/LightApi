using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using MongoDB.Entities;

namespace LightApi.Infra.FileStorage;

public class FileStorage : IFileStorage
{
    private readonly IOptions<StorageOptions> _options;
    private readonly IMinioClient? _minioClient;

    public FileStorage(IOptions<StorageOptions> options, IMinioClient? minioClient = null)
    {
        if (options.Value.MinioStorageOptions != null && minioClient == null)
        {
            throw new ArgumentNullException("当设置了MinioStorageOptions时，必须注册IMinioClient服务");
        }

        _options = options;
        _minioClient = minioClient;
    }

    public Task<string> UploadToLocalStorage(IFormFile file)
    {
        return UploadToLocalStorage(file.OpenReadStream(), file.FileName);
    }

    public async Task<string> UploadToLocalStorage(Stream stream, string fileName)
    {
        string subDir = DateTime.Now.ToString("yyMMdd");
        var rootDir = Path.Combine(GetAbsoluteDirectory(), subDir);
        if (!Directory.Exists(rootDir))
            Directory.CreateDirectory(rootDir);
        fileName = $"{DateTime.Now:yyMMddhhmmssfff}_{fileName}";
        string filePath = Path.Combine(rootDir, fileName);
        await using Stream fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        return $"{_options.Value.LocalStorageOptions!.PublicDomain}/{subDir}/{fileName}";
    }

    public FileStream? DownloadFromLocalStorage(string subPath)
    {
        var rootDir = GetAbsoluteDirectory();
        var filePath = Path.Combine(rootDir, subPath);
        return File.Exists(filePath) == false ? null : File.OpenRead(filePath);
    }

    public Task<string> UploadToMinioStorage(IFormFile file)
    {
        return UploadToMinioStorage(file.OpenReadStream(), file.FileName);
    }

    public async Task<string> UploadToMinioStorage(Stream stream, string fileName)
    {
        if (_minioClient == null)
            throw new InvalidOperationException("Minio client is not initialized");

        string objectKey = $"{DateTime.Now:yyMMdd}/{DateTime.Now:yyMMddhhmmssfff}_{fileName}";

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Value.MinioStorageOptions!.Bucket)
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length);
        var uploadResponse = await _minioClient.PutObjectAsync(putObjectArgs);
        int statusInt = (int)uploadResponse.ResponseStatusCode;
        if (statusInt is < 200 or > 299)
        {
            throw new MinioException($"文件上传失败:[{statusInt}]{uploadResponse.ResponseContent}");
        }
        string prefix = _options.Value.MinioStorageOptions.EnableSSL ? "https" : "http";
        return $"{prefix}://{_options.Value.MinioStorageOptions.EndPoint}/{_options.Value.MinioStorageOptions.Bucket}/{objectKey}";
    }

    public async Task<string?> UploadToMongoDBStorage(
        Stream stream,
        string fileName,
        bool isTempFile = false
    )
    {
        var file = new MongoStorage() { FileName = fileName, IsTempFile = isTempFile };
        await file.SaveAsync();
        await file.Data.UploadAsync(stream);
        return $"{_options.Value.MongoStorageOptions!.PublicDomain}/{file.ID}_{file.FileName}";
    }

    public async Task<Stream?> DownloadFromMongoDBStorage(string key)
    {
        try
        {
            if (key.Length > 24)
                key = key.Split("_")[0];
            if (key.Length != 24)
                return null;
            var mongoFile = await DB.Find<MongoStorage>()
                .Match(it => it.ID == key)
                .ExecuteFirstAsync();

            if (mongoFile != null)
            {
                var memoryStream = new MemoryStream();
                await mongoFile.Data.DownloadAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 生成Minio预签名的上传链接
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<string> GenerateMinioUploadUrl(string fileName)
    {
        if (_minioClient == null)
            throw new InvalidOperationException("Minio client is not initialized");

        string objectKey = $"{DateTime.Now:yyMMdd}/{DateTime.Now:yyMMddhhmmssfff}_{fileName}";
        var args = new PresignedPutObjectArgs()
            .WithBucket(_options.Value.MinioStorageOptions!.Bucket)
            .WithObject(objectKey)
            .WithExpiry(1800); // 30分钟 = 1800秒

        return await _minioClient.PresignedPutObjectAsync(args);
    }

    /// <summary>
    /// 对于本地存储，获取绝对路径
    /// </summary>
    /// <returns></returns>
    private string GetAbsoluteDirectory()
    {
        return Path.IsPathRooted(_options.Value.LocalStorageOptions!.Directory) == false
            ? Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                _options.Value.LocalStorageOptions.Directory
            )
            : _options.Value.LocalStorageOptions.Directory;
    }
}
