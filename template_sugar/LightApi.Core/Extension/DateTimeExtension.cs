namespace LightApi.Core.Extension;

public static class DateTimeExtension
{
    /// <summary>
    /// 获取下个月的第一天时间 例如 现在是2023-01-15日，那么获取的下个月是2023-02-01 00:00:00
    /// </summary>
    /// <returns></returns>
    public static DateTime GetNextMonth(this DateTime now)
    {
        var nextMonth = now.AddMonths(1);
        var nextMonthFirstDay = new DateTime(nextMonth.Year, nextMonth.Month, 1);
        // set kind to local
        nextMonthFirstDay = DateTime.SpecifyKind(nextMonthFirstDay, DateTimeKind.Local);
        return nextMonthFirstDay;
    }
    
    /// <summary>
    /// 获取下个月的第一天时间 例如 现在是2023-01-15日，那么获取的下个月是2023-02-01 00:00:00
    /// </summary>
    /// <returns></returns>
    public static DateTime GetNextDay(this DateTime now)
    {
        var nextDay = now.AddDays(1);
        var nextDayFirstDay = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day);
        // set kind to local
        nextDayFirstDay = DateTime.SpecifyKind(nextDayFirstDay, DateTimeKind.Local);
        return nextDayFirstDay;
    }
}