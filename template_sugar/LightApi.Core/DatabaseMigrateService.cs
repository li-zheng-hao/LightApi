using System.Reflection;
using EvolveDb;
using LightApi.Domain;
using LightApi.SqlSugar;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SqlSugar;

namespace LightApi.Core;

public class DatabaseMigrateService:IHostedService
{
    private readonly ISqlSugarClient _client;

    public DatabaseMigrateService(ISqlSugarClient client)
    {
        _client = client;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Type[] types= Assembly
            .LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LightApi.Domain.dll"))//如果 .dll报错，可以换成 xxx.exe 有些生成的是exe 
            .GetTypes().Where(it=>typeof(ISugarTable).IsAssignableFrom(it))//命名空间过滤，当然你也可以写其他条件过滤
            .ToArray();//断点调试一下是不是需要的Type，不是需要的在进行过滤
        
        var diffString= _client.CodeFirst.GetDifferenceTables(types).ToDiffString();
        Log.Information($"本次数据库迁移差距:{diffString}");
        _client.CodeFirst.SetStringDefaultLength(200).InitTables(types);//这样一个表就能成功创建了
        
        Log.Information("数据库迁移完成");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}