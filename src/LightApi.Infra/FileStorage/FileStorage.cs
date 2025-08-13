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
        string fileExt = Path.GetExtension(fileName);

        string objectKey = $"{DateTime.Now:yyMMdd}/{Guid.NewGuid():N}{fileExt}";
        var headers = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Original-Filename", fileName },
        };
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Value.MinioStorageOptions!.Bucket)
            .WithObject(objectKey)
            .WithHeaders(headers)
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
            var mongoFile = await DB.Find<MongoStorage>()
                .Match(it => it.ID == GetMongoDBStorageKey(key))
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

    public async Task<bool> DeleteFromMongoDBStorage(string key)
    {
        var mongoFile = await DB.Find<MongoStorage>()
            .Match(it => it.ID == GetMongoDBStorageKey(key))
            .ExecuteFirstAsync();
        if (mongoFile == null)
            return false;
        var result = await mongoFile.DeleteAsync();
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    private string GetMongoDBStorageKey(string key)
    {
        if (key.StartsWith(_options.Value.MongoStorageOptions!.PublicDomain))
        {
            // 移除PublicDomain/
            key = key.Substring(_options.Value.MongoStorageOptions!.PublicDomain.Length + 1);
        }
        if (key.Length > 24)
            key = key.Split("_")[0];
        return key;
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
        string fileExt = Path.GetExtension(fileName);

        string objectKey = $"{DateTime.Now:yyMMdd}/{Guid.NewGuid():N}{fileExt}";
        var headers = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Original-Filename", fileName },
        };
        var args = new PresignedPutObjectArgs()
            .WithBucket(_options.Value.MinioStorageOptions!.Bucket)
            .WithObject(objectKey)
            .WithHeaders(headers)
            .WithExpiry(1800); // 30分钟 = 1800秒

        return await _minioClient.PresignedPutObjectAsync(args);
    }

    /// <summary>
    /// 生成Minio分享下载的链接
    /// </summary>
    /// <param name="objectKey"></param>
    /// <param name="expireSeconds"></param>
    /// <returns></returns>
    public async Task<string> GenerateMinioDownloadUrl(string objectKey, int expireSeconds = 1800)
    {
        if (_minioClient == null)
            throw new InvalidOperationException("Minio client is not initialized");
        var args = new PresignedGetObjectArgs()
            .WithBucket(_options.Value.MinioStorageOptions!.Bucket)
            .WithObject(objectKey)
            .WithExpiry(expireSeconds); // 30分钟 = 1800秒
        // 生成预签名 URL
        string url = await _minioClient!.PresignedGetObjectAsync(args);
        return url;
    }

    /// <summary>
    /// 从当前FileStorage minio生成的url获取objectKey
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public string GetMinioObjectKeyFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL不能为空", nameof(url));

        try
        {
            // 解析URL，获取路径部分
            var uri = new Uri(url);
            var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // 路径格式应该是: /{bucket}/{objectKey}
            // 如果路径段少于2个，说明URL格式不正确
            if (pathSegments.Length < 2)
                throw new ArgumentException("无效的Minio URL格式", nameof(url));

            // 第一个段是bucket，第二个段开始是objectKey
            var bucket = pathSegments[0];
            var objectKey = string.Join("/", pathSegments.Skip(1));

            // 验证bucket是否匹配配置
            if (
                _options.Value.MinioStorageOptions?.Bucket != null
                && bucket != _options.Value.MinioStorageOptions.Bucket
            )
            {
                throw new ArgumentException(
                    $"URL中的bucket '{bucket}' 与配置的bucket '{_options.Value.MinioStorageOptions.Bucket}' 不匹配",
                    nameof(url)
                );
            }

            return objectKey;
        }
        catch (UriFormatException)
        {
            throw new ArgumentException("无效的URL格式", nameof(url));
        }
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
