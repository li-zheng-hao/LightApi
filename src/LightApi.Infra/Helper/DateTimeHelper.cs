namespace LightApi.Infra.Helper;

public static class DateTimeHelper
{
    /// <summary>
    /// 获取对应时间所在周的指定星期几的日期
    /// </summary>
    /// <param name="currentTime"></param>
    /// <param name="indexOfWeek">1-7 从星期一到星期天</param>
    /// <returns></returns>
    public static DateTime GetTimeOfWeek(DateTime currentTime, int indexOfWeek)
    {
        if (indexOfWeek < 1 || indexOfWeek > 7)
            throw new ArgumentOutOfRangeException(nameof(indexOfWeek), "indexOfWeek must be between 1 and 7");

        int dayOfWeek = (int)currentTime.DayOfWeek;
        dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
        int dayDiff = indexOfWeek - dayOfWeek;

        return currentTime.AddDays(dayDiff).Date;
    }
}