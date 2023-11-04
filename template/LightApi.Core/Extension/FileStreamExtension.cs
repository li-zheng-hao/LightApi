namespace LightApi.Core.Extension;

public static class FileStreamExtension
{

    /// <summary>
    /// 获取文件流的字节数组
    /// </summary>
    /// <param name="fileStream"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this FileStream fileStream)
    {
        using var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}