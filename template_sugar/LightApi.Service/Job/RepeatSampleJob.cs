using LightApi.Core.Autofac;
using Serilog;

namespace LightApi.Service.Job;

public class RepeatSampleJob:IScopedDependency
{
    private readonly StudentService _studentService;
    
    public RepeatSampleJob(StudentService studentService)
    {
        _studentService = studentService;
    }
    
    public async Task ExecuteAsync()
    {
        await Task.Delay(10000);
        Log.Information("RepeatSampleJob ExecuteAsync");
    }
}