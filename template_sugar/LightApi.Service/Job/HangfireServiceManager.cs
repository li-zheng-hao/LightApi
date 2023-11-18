using Hangfire;
using LightApi.Core.Autofac;
using LightApi.Core.Job;

namespace LightApi.Service.Job;

/// <summary>
/// 
/// </summary>
public class HangfireServiceManager:IJobManager,ISingletonDependency
{
    /// <summary>
    /// 
    /// </summary>
    public void Register()
    {
        RecurringJob.AddOrUpdate<RepeatSampleJob>(nameof(RepeatSampleJob),it=>it.ExecuteAsync(),Cron.Minutely); 

    }
}