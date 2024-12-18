using LightApi.Infra.FileStorage;
using LightApi.Mongo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace LightApi.UnitTest;

/// <summary>
/// 存储测试
/// </summary>
public class StorageTest
{
    private string _testFile = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Files",
        "test_excel.xlsx"
    );

    [Fact]
    public async Task MongoStorageTest()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddMongoSetup(
            "test",
            Environment.GetEnvironmentVariable("APP_MONGO_CONNECTIONSTRING")
        );
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var storageOptions = new StorageOptions()
        {
            MongoStorageOptions = new MongoStorageOptions()
            {
                PublicDomain = "http://localhost/api/file",
            },
        };
        IOptions<StorageOptions> options = new OptionsWrapper<StorageOptions>(storageOptions);
        var fileStorage = new FileStorage(options);
        await using var fileStream = File.OpenRead(_testFile);
        var file = new FormFile(fileStream, 0, fileStream.Length, "test.xlsx", "test.xlsx");
        var res = await fileStorage.UploadToMongoDBStorage(file.OpenReadStream(), "test.xlsx");
        Assert.NotNull(res);
        res = res.Replace($"{storageOptions.MongoStorageOptions.PublicDomain}/", "");
        var res2 = await fileStorage.DownloadFromMongoDBStorage(res);
        Assert.NotNull(res2);
    }

    [Fact]
    public async Task LocalStorageTest()
    {
        var storageOptions = new StorageOptions()
        {
            LocalStorageOptions = new LocalStorageOptions()
            {
                PublicDomain = "http://localhost/api",
                Directory = "Storage"
            },
        };

        IOptions<StorageOptions> options = new OptionsWrapper<StorageOptions>(storageOptions);
        var fileStorage = new FileStorage(options);
        await using var fileStream = File.OpenRead(_testFile);
        var file = new FormFile(fileStream, 0, fileStream.Length, "test.xlsx", "test.xlsx");
        var res = await fileStorage.UploadToLocalStorage(file);
        Assert.NotNull(res);
    }

    [Fact]
    public async Task MinioStorageTest()
    {
        var storageOptions = new StorageOptions()
        {
            MinioStorageOptions = new MinioStorageOptions()
            {
                EndPoint = "",
                AccessKey = "",
                SecretKey = "",
                Bucket = "",
                EnableSSL = false
            }
        };
        IMinioClient? client = null;
        if (storageOptions.MinioStorageOptions != null)
        {
            client = new MinioClient()
                .WithEndpoint(storageOptions.MinioStorageOptions.EndPoint)
                .WithCredentials(
                    storageOptions.MinioStorageOptions.AccessKey,
                    storageOptions.MinioStorageOptions.SecretKey
                )
                .WithSSL(storageOptions.MinioStorageOptions.EnableSSL)
                .Build();
        }

        IOptions<StorageOptions> options = new OptionsWrapper<StorageOptions>(storageOptions);
        using var fs = File.OpenRead(_testFile);
        var fileStorage = new FileStorage(options, client);
        var res = await fileStorage.UploadToMinioStorage(fs, "test.xlsx");
        Assert.NotNull(res);
    }
}
