using System.Data;

namespace LightApi.Infra.Extension;

public static class DataTableExtension
{
    /// <summary>
    /// 新增一行数据
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="rowValues"></param>
    public static void AddRow(this DataTable dataTable, params object[] rowValues)
    {
        dataTable.Rows.Add(rowValues);
    }
}
