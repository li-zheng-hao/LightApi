using System.IO.Compression;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using SevenZipExtractor;

namespace LightApi.Core.Helper;

public static class FileHelper
{
    public static string GetMD5(Stream stream)
    {
        using var md5 = MD5.Create();
        stream.Position = 0;
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// 删除 不存在则忽略
    /// </summary>
    /// <param name="path"></param>
    public static void Delete(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
    /// <summary>
    /// 递归删除 不存在则忽略
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteDirectory(string path,bool recursive=true)
    {
        if (Directory.Exists(path))
            Directory.Delete(path,recursive);
    }

    /// <summary>
    /// 从IFormFileCollection中获取指定文件名的索引 如果不存在则返回-1
    /// </summary>
    /// <param name="files"></param>
    /// <param name="fileName"></param>
    /// <param name="IgnoreCase">是否忽略大小写比较</param>
    /// <param name="ignoreExtension">是否忽略后缀名</param>
    /// <returns></returns>
    public static int GetIndexFromIFormFileCollection(IFormFileCollection files, string fileName,
        bool IgnoreCase = false, bool ignoreExtension = true)
    {
        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            // Perform your desired check or condition
            if (ignoreExtension)
            {
                if (Path.GetFileNameWithoutExtension(file.FileName).Equals(Path.GetFileNameWithoutExtension(fileName),
                        IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    return i;
                }
            }
            else if (file.FileName.Equals(fileName,
                         IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    public static List<(string fileName, Stream stream)> DecompressFilesBy7Zip(Stream inputStream,
        params string[] exts)
    {
        List<(string fileName, Stream stream)> result = new();
        using var archiveFile = new ArchiveFile(inputStream);
        foreach (var entry in archiveFile.Entries)
        {
            if (!exts.Contains(Path.GetExtension(entry.FileName), StringComparer.OrdinalIgnoreCase))
                continue;
            var memoryStream = new MemoryStream();
            entry.Extract(memoryStream);
            result.Add((entry.FileName, memoryStream));
        }
        return result;
    }


    public static Stream CompressFiles(List<(Stream Stream, string fileName)> files)
    {
        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var zipArchiveEntry = archive.CreateEntry(file.fileName);
                using var zipStream = zipArchiveEntry.Open();
                file.Stream.CopyTo(zipStream);
            }
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}