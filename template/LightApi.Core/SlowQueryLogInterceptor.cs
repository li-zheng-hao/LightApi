using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

namespace LightApi.Core;

public class SlowQueryLogInterceptor:DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {

        if (eventData.Duration.TotalMilliseconds > 500)
        {
            Log.Warning("数据库查询耗时过长 查询语句: {CommandCommandText} 耗时时间 {DurationTotalMilliseconds}", command.CommandText, eventData.Duration.TotalMilliseconds);
        }
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Duration.TotalMilliseconds > 500)
        {
            Log.Warning("数据库查询耗时过长 查询语句: {CommandCommandText} 耗时时间 {DurationTotalMilliseconds}", command.CommandText, eventData.Duration.TotalMilliseconds);
        }
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }
}