using System.Diagnostics;
using Serilog;

namespace LightApi.Core.Helper;

public static class ProcessorHelper
{
    /// <summary>
    /// 调用外部进程 注意路径不能有空格
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="ignoreException">是否需要忽略调用过程的异常，默认false</param>
    /// <param name="waitSeconds">超时秒数</param>
    public async static Task InvokeAsync(string cmd, bool ignoreException = false, int waitSeconds = 600)
    {
        try
        {
            var cmds = cmd.Split(" ");
            Process process = new Process();
            // 设置工作文件夹
            process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            process.StartInfo.FileName = cmds[0];
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.Arguments = string.Join(" ", cmds.Skip(1));
            process.Start();

            Task.Run(() =>
            {
                try
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line = process.StandardOutput.ReadLine();
                        // do something with line
                        Log.Information(line);
                    }
                }
                catch (Exception)
                {
                }
            });

            Task.Run(() =>
            {
                try
                {
                    while (!process.StandardError.EndOfStream)
                    {
                        string line = process.StandardError.ReadLine();
                        // do something with line
                        Log.Error(line);
                    }
                }
                catch (Exception)
                {
                }
            });

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(waitSeconds * 1000);

            await process.WaitForExitAsync(cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            //https://stackoverflow.com/questions/61935980/process-finished-with-exit-code-1073740940-0xc0000374
            if (ignoreException)
            {
                Log.Error(ex, "进程调用, 此异常已被忽略");

                return;
            }

            Log.Error(ex, "进程调用异常");

            throw;
        }
    }

    /// <summary>
    /// 原生调用外部进程 路径可以有空格
    /// </summary>
    /// <param name="exe"></param>
    /// <param name="param"></param>
    /// <param name="ignoreException"></param>
    /// <param name="waitSeconds"></param>
    public static async Task InvokeAsync(string exe, string param, bool ignoreException = false, int waitSeconds = 600)
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = exe;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.Arguments = param;
            process.EnableRaisingEvents = false;

            process.Start();

            var task1=Task.Run(() =>
            {
                try
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string? line = process.StandardOutput.ReadLine();
                        // do something with line
                        if (line != null) Log.Information(line);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            });

            var task2=Task.Run(() =>
            {
                try
                {
                    while (!process.StandardError.EndOfStream)
                    {
                        string? line = process.StandardError.ReadLine();
                        // do something with line
                        if (line != null) Log.Error(line);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            });

            var cancellationTokenSource = new CancellationTokenSource(waitSeconds * 1000);

            var task3= process.WaitForExitAsync(cancellationTokenSource.Token);
            await Task.WhenAll(task1, task2, task3);
        }
        catch (Exception ex)
        {
            //https://stackoverflow.com/questions/61935980/process-finished-with-exit-code-1073740940-0xc0000374
            if (ignoreException)
            {
                Log.Error(ex, "进程调用, 此异常已被忽略");

                return;
            }

            Log.Error(ex, "进程调用异常");

            throw;
        }
    }
}