using LightApi.Core.FileProvider;
using LightApi.Core.Helper;
using LightApi.Core.Options;
using Masuit.Tools;
using Microsoft.AspNetCore.Http;
using Minio;
using Serilog;

namespace PDM.Core.FileProvider;

public class MinioFileProvider : IFileProvider
{
    private readonly string? _secretKey;
    private readonly string? _accessKey;
    private readonly string? _bucket;
    private MinioClient? _minioClient;
    private readonly string? _publicDomain;
    public string RootDir { get; set; }
    public FileNameGenerateStrategy FileNameGenerateStrategy { get; set; }


    public MinioFileProvider(MinIOStorage storage)
    {
        // 内网地址
        RootDir =  storage.Endpoint;
        _bucket = storage.Bucket;
        _accessKey = storage.AccessKey;
        _secretKey = storage.SecretKey;
        // 公网地址
        _publicDomain = storage.PublicDomain;
        FileNameGenerateStrategy=storage.FileNameGenerateStrategy;
        InitMinioClient();
    }

    private void InitMinioClient()
    {
        try
        {
            var entpoint = RootDir.StartsWith("https")
                ? RootDir.Replace("https://","")
                : RootDir.Replace("http://","");
            _minioClient = new MinioClient()
                .WithEndpoint(entpoint)
                .WithCredentials(_accessKey, _secretKey)
                .Build();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Minio客户端连接失败");
            _minioClient = null;
        }
    }

    public async Task<(string? fileUrl, string? md5)> SaveFile(Stream stream, string fileName)
    {
        if (_minioClient == null) return default;
        
        var md5 = FileHelper.GetMD5(stream);

        stream.Position = 0;
        fileName = md5 + Path.GetExtension(fileName);

        var beArgs = new BucketExistsArgs()
            .WithBucket(_bucket);
        bool found = await _minioClient.BucketExistsAsync(beArgs);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(_bucket);
            await _minioClient.MakeBucketAsync(mbArgs);
        }

        long length = stream.Length;
        stream.Seek(0, SeekOrigin.Begin);
        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(length)
            ;

        await _minioClient.PutObjectAsync(putObjectArgs);

        return ($"{_publicDomain}/{_bucket}/{fileName}", md5);
    }

    public Task<(string? fileUrl, string? md5)> SaveFile(IFormFile file)
    {
        return SaveFile(file.OpenReadStream(), fileName: file.FileName);
    }

    public Task<(string? fileUrl, string? md5)> SaveFile(byte[] fileBytes, string fileName)
    {
        if (_minioClient == null) return default;
        MemoryStream ms = new MemoryStream(fileBytes);
        return SaveFile(ms, fileName);
    }

    public async Task<Stream?> GetStream(string fileUrl)
    {
        if (_minioClient == null) return default;

        fileUrl=ReplacePublicUrlToInernal(fileUrl);

        
        var prefix = $"{RootDir}/{_bucket}/".Length;
        var objectKey = fileUrl.Substring(prefix);

        Stream ms = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey)
            .WithCallbackStream((stream, token) =>
            {
                stream.CopyTo(ms);
                return Task.CompletedTask;
            });
        await _minioClient.GetObjectAsync(getObjectArgs);
        ms.Position = 0;
        return ms;
    }

    public async Task<byte[]?> GetFileBytes(string fileUrl)
    {
        if (_minioClient == null) return default;

        fileUrl=ReplacePublicUrlToInernal(fileUrl);

        
        var prefix = $"{RootDir}/{_bucket}/".Length;
        var objectKey = fileUrl.Substring(prefix);

        Stream ms = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey)
            .WithCallbackStream((stream, token) =>
            {
                stream.CopyTo(ms);
                return Task.CompletedTask;
            });
        await _minioClient.GetObjectAsync(getObjectArgs);
        return await ms.ToArrayAsync();
    }

    public async Task DeleteFile(string fileUrl)
    {
        if (_minioClient == null) return;
        
        fileUrl=ReplacePublicUrlToInernal(fileUrl);
        
        var prefix = $"{RootDir}/{_bucket}/".Length;
        var objectKey = fileUrl.Substring(prefix);

        var args = new RemoveObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey);
        await _minioClient.RemoveObjectAsync(args);
    }
    
    /// <summary>
    /// 将公网地址替换为内网地址
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public string ReplacePublicUrlToInernal(string url)
    {
        return url.Replace(_publicDomain,RootDir);
    }
}