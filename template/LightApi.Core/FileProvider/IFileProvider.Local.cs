using LightApi.Core.Extension;
using LightApi.Core.Helper;
using Masuit.Tools;
using Microsoft.AspNetCore.Http;

namespace LightApi.Core.FileProvider;

public class LocalFileProvider : IFileProvider
{
    private string _rootDir;

    public string RootDir
    {
        get => _rootDir;
        set
        {
            _rootDir = value;
            Directory.CreateDirectory(RootDir);
        }
    }

    public FileNameGenerateStrategy FileNameGenerateStrategy { get; set; }

    public async Task<(string? fileUrl, string? md5)> SaveFile(Stream stream, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        
        string fileNameWithoutExt=string.Empty;
        
        var res=CreateFileName(stream, fileName);

        var path = Path.Combine(RootDir, res.filePath);
        if (!File.Exists(path))
        {
            await Write(stream, path);
        }

        return (res.filePath, res.md5);
    }

    private (string filePath,string md5) CreateFileName(Stream fileStream,string? fileName=null)
    {
        string fileNameWithoutExt = string.Empty;
        string md5 = string.Empty;
        md5 = FileHelper.GetMD5(fileStream);
        switch (FileNameGenerateStrategy) 
        {
            case FileNameGenerateStrategy.TimeStampWithDayPrefix:
            {
                var subDir=Path.Combine(RootDir, DateTime.Today.ToString("yyyyMMdd"));
                Directory.CreateDirectory(subDir);
                fileNameWithoutExt =Path.Combine(subDir.Replace(RootDir,""),$"{DateTime.Now.Ticks.ToString()}_{fileName}");
                break;
            }
            case  FileNameGenerateStrategy.MD5 :
            {
                fileNameWithoutExt = md5;
                break;
            }
        };
        if(fileNameWithoutExt.StartsWith("/")||fileNameWithoutExt.StartsWith("\\"))
            fileNameWithoutExt=fileNameWithoutExt[1..];
        return (fileNameWithoutExt!,md5);
    }
    private async Task Write(Stream stream, string filePath)
    {
        using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        byte[] array = stream.ToArray();
        await fileStream.WriteAsync(array, 0, array.Length);
        await fileStream.FlushAsync();
    }
    public async Task<(string? fileUrl, string? md5)> SaveFile(IFormFile file)
    {
        var stream = file.OpenReadStream();

        return await SaveFile(stream, file.FileName);
    }

    public Task<(string? fileUrl, string? md5)> SaveFile(byte[] fileBytes, string fileName)
    {
        MemoryStream ms = new MemoryStream(fileBytes);

        return SaveFile(ms, fileName);
    }


    public async Task<Stream?> GetStream(string fileUrl)
    {
        var path = Path.Combine(RootDir, fileUrl);

        return File.Exists(path) ? File.OpenRead(path) : throw new FileNotFoundException($"{fileUrl} not found");
    }

    public Task<byte[]?> GetFileBytes(string fileUrl)
    {
        var path = Path.Combine(RootDir, fileUrl);

        using var fileStream = File.Exists(path)
            ? File.OpenRead(path)
            : throw new FileNotFoundException($"{fileUrl} not found");

        return Task.FromResult(fileStream.GetBytes());
    }

    public async Task DeleteFile(string fileUrl)
    {
        var path = Path.Combine(RootDir, fileUrl);

        if (!File.Exists(path))
            return ;

        File.Delete(path);
    }
}