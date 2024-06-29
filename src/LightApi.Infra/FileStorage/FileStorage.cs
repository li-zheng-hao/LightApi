using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace LightApi.Infra.FileStorage;

public class FileStorage:IFileStorage
{
    private readonly IOptions<StorageOptions> _options;
    private readonly IMinioClient? _minioClient;

    public FileStorage(IOptions<StorageOptions> options,IMinioClient? minioClient=null)
    {
        if (options.Value.MinioStorageOptions != null&& minioClient==null)
        {
            throw new ArgumentNullException("当设置了MinioStorageOptions时，必须注册IMinioClient服务");
        }

        _options = options;
        _minioClient = minioClient;
    }
    public  Task<string> UploadToLocalStorage(IFormFile file)
    {
        return UploadToLocalStorage(file.OpenReadStream(), file.FileName);
    }

    public async Task<string> UploadToLocalStorage(Stream stream, string fileName)
    {
        string subDir = DateTime.Now.ToString("yyMMdd");
        var rootDir=Path.Combine(GetAbsoluteDirectory(),subDir);
        if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);
        fileName = $"{DateTime.Now:yyMMddhhmmssfff}_{fileName}";
        string filePath = Path.Combine(rootDir,fileName );
        await using Stream fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        return $"{_options.Value.LocalStorageOptions!.PublicDomain}/{subDir}/{fileName}";
    }

    public FileStream? DownloadFromLocalStorage(string subPath)
    {
        var rootDir=GetAbsoluteDirectory();
        var filePath = Path.Combine(rootDir,subPath);
        return File.Exists(filePath) == false ? null : File.OpenRead(filePath);
    }

    public Task<string?> UploadToMinioStorage(IFormFile file)
    {
        return UploadToMinioStorage(file.OpenReadStream(), file.FileName);
    }

    public async Task<string?> UploadToMinioStorage(Stream stream, string fileName)
    {
        if (_minioClient == null) return null;

        string objectKey = $"{DateTime.Now:yyMMdd}/{DateTime.Now:yyMMddhhmmssfff}_{fileName}";
      
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Value.MinioStorageOptions!.Bucket)
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            ;
        
        await _minioClient.PutObjectAsync(putObjectArgs);
        string prefix = _options.Value.MinioStorageOptions.EnableSSL ? "https" : "http";
        return $"{prefix}://{_options.Value.MinioStorageOptions.EndPoint}/{_options.Value.MinioStorageOptions.Bucket}/{objectKey}";
    }

    /// <summary>
    /// 对于本地存储，获取绝对路径
    /// </summary>
    /// <returns></returns>
    private string GetAbsoluteDirectory()
    {
        return Path.IsPathRooted(_options.Value.LocalStorageOptions!.Directory)==false ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _options.Value.LocalStorageOptions.Directory) : _options.Value.LocalStorageOptions.Directory;
        
    }
}