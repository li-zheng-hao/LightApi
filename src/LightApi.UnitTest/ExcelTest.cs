using LightApi.Infra.Helper;
using Masuit.Tools.Files;

namespace LightApi.UnitTest;

public class ExcelTest
{
    [Fact]
    public void ImportExcel()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "test2.xlsx");
        using var fs = File.OpenRead(path);
        var res = ExcelHelper.GetValuesByNpoi(fs);
        Assert.True(res.Rows.Count == 5);
        ExcelHelper.RemoveAllEmptyRow(res);
        Assert.True(res.Rows.Count == 4);
    }
}
