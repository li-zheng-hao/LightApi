using LightApi.Core.Extension;
using LightApi.Infra.Extension;
using Masuit.Tools;
using MiniExcelLibs;

namespace LightApi.Core.Helper;

public static class MiniExcelHelper
{
    /// <summary>
    /// 检查头是否合理
    /// </summary>
    /// <param name="excelStream"></param>
    /// <param name="useHeaders"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static bool ValidateHeaders(Stream excelStream, bool useHeaders, params string[] headers)
    {
        var columns = excelStream.GetColumns(useHeaders);

        // 顺序比较相等
        return columns.SequenceEqual(headers);
    }

    /// <summary>
    /// 检查头是否合理
    /// </summary>
    /// <param name="excelStream"></param>
    /// <param name="useHeaders"></param>
    /// <param name="startCell"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static bool ValidateHeadersWithStartCell(Stream excelStream, bool useHeaders,string startCell, params string[] headers)
    {
        var columns = excelStream.GetColumns(useHeaders,startCell:startCell);

        // 顺序比较相等
        return columns.SequenceEqual(headers);
    }
    /// <summary>
    /// 检查头是否合理
    /// </summary>
    /// <param name="excelStream"></param>
    /// <param name="useHeaders"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static bool ValidateHeaders(Stream excelStream,string startCell, bool useHeaders, params string[] headers)
    {
        var columns = excelStream.GetColumns(useHeaders,startCell:startCell);

        // 顺序比较相等
        return columns.SequenceEqual(headers);
    }
    public async static Task<List<Dictionary<string, string>>> GetValues(Stream excelStream, bool useHeaderRow = true, string startCell = "A1")
    {
        var rows = (await excelStream.QueryAsync(useHeaderRow: useHeaderRow, startCell: startCell))
            .Cast<IDictionary<string, object>>().ToList();

        // convert rows to Dictionary<string,string>
        var values = rows.Select(it => it.ToDictionary(k => k.Key
            , v => v.Value.ToString()?.Trim())).ToList();

        // remove empty row
        values.RemoveAll(it => it.All(item => string.IsNullOrWhiteSpace(item.Value)));
        
        return values;
    }

    /// <summary>
    /// 检查是否有空行
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool CheckIfExistsEmptyRow(List<IDictionary<string, string>> values)
    {
        return values.Any(it => it.All(item => string.IsNullOrWhiteSpace(item.Value)));
    }

    /// <summary>
    /// 检查这一列是否有不能转换成数字的值
    /// </summary>
    /// <param name="values"></param>
    /// <param name="colName"></param>
    /// <returns></returns>
    public static bool CheckIfExistNoNumberValue(List<Dictionary<string, string>> values,string colName)
    {
        return !values.All(it => it.ContainsKey(colName) && double.TryParse(it[colName],out _));
    }

    /// <summary>
    /// 删除最后一行 如果不存在则忽略
    /// </summary>
    /// <param name="data"></param>
    public static void  RemoveLastRow(List<Dictionary<string, string>> data)
    {
        if (!data.Any()) return;
        
        var lastRow = data.Count()-1;
        data.RemoveAt(lastRow);
    }

    /// <summary>
    /// 检查某一列是否为空 是的话抛出异常
    /// </summary>
    /// <param name="data"></param>
    /// <param name="colName"></param>
    /// <returns></returns>
    public static bool CheckIfEmptyColumn(List<Dictionary<string, string>> data, string colName)
    {
        var exist=data.Any(it => it.ContainsKey(colName) && it[colName].IsNullOrWhiteSpace());
        return exist;
    }
    /// <summary>
    /// 检查某一列是否存在不为Int类型或者空白的数据 是的话抛出异常
    /// </summary>
    /// <param name="data"></param>
    /// <param name="colName"></param>
    /// <returns></returns>
    public static bool CheckIfNotIntOrEmpty(List<Dictionary<string, string>> data, string colName)
    {
        var exist=data.Any(it => it.ContainsKey(colName) && !it[colName].IsNullOrWhiteSpace() && !it[colName].IsInt());
        return exist;
    }
    /// <summary>
    /// 检查某一列是否存在不为数字的数据 double类型 是的话抛出异常
    /// </summary>
    /// <param name="data"></param>
    /// <param name="colName"></param>
    /// <returns></returns>
    public static bool CheckIfNotNumber(List<Dictionary<string, string>> data, string colName)
    {
        var exist=CheckIfExistNoNumberValue(data, colName);
        return exist;
    }
    
    /// <summary>
    /// 检查某一列是否存在不为正整数的数据 是的话返回false
    /// </summary>
    /// <param name="data"></param>
    /// <param name="colName"></param>
    /// <returns></returns>
    public static bool CheckIfNotPositiveInt(List<Dictionary<string, string>> data, string colName)
    {
        var exist=data.Any(it => it.ContainsKey(colName) && !it[colName].IsNullOrWhiteSpace() && !it[colName].IsPositiveInt());
        return exist;
    }
    /// <summary>
    /// 检查是否为空表
    /// </summary>
    /// <param name="data"></param>
    /// <param name="colName"></param>
    /// <returns></returns>
    public static bool CheckIfEmptyTable(List<Dictionary<string, string>> data)
    {
        return data.IsNullOrEmpty();
    }
}