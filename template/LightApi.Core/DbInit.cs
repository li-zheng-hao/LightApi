using EvolveDb;
using LightApi.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightApi.Core;

public class DbInit
{
    /// <summary>
    /// 数据库迁移 EFCore和Evolve
    /// </summary>
    /// <param name="app"></param>
    public static void Init(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FbAppContext>();
        // 如果多实例 这里需要加redis锁 或者把迁移拆分出来
        db.Database.Migrate();

        var connection = db.Database.GetDbConnection();
        
        var evolve = new Evolve(connection, msg => Log.Information(msg))
        {
            Locations = new[] { "SqlMigrations" },
            IsEraseDisabled = true
        };
        
        evolve.Migrate();
    }
}