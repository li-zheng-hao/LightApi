using Newtonsoft.Json;

namespace LightApi.Core.Helper;

public class ProcessMessageHelper
{
    /// <summary>
    /// 基本目录
    /// </summary>
    public static string BasePath { get; set; }
        =Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ProcessMessages");

    /// <summary>
    /// 生成的文件目录
    /// </summary>
    /// <param name="data"></param>
    /// <param name="id">文件路径</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>文件路径</returns>
    public static string Write<T>(T data,string path="")
    {
        var fileName = $"{Guid.NewGuid().ToString()}.json";
        var filePath = Path.Combine(BasePath, fileName);
        
        if (!string.IsNullOrWhiteSpace(path))
        {
            filePath = Path.Combine(BasePath, path);
        }

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        
        var dataStr=JsonConvert.SerializeObject(data);
        
        Console.Write(filePath);
        
        File.WriteAllText(filePath,dataStr);
        
        return filePath;
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="deleteAfterRead">读取完成后自动删除</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? Read<T>(string path,bool deleteAfterRead=true)
    {
        try
        {
            var dataStr = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(dataStr);
        }
        finally
        {
            if(File.Exists(path)&&deleteAfterRead)
                File.Delete(path);
        }
       
    }


    public static TResult? Execute<TParam,TResult>(string cmd,TParam param)
    {
        var paramPath = Write(param);
        var process = ProcessorHelper.InvokeAsync(cmd+$" {paramPath}");
        process?.Wait();
        return Read<TResult>(paramPath);
    }
}