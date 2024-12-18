using System.Data;
using System.Globalization;
using LightApi.Infra.Extension;
using LightApi.Infra.InfraException;
using MiniExcelLibs;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace LightApi.Infra.Helper;

/// <summary>
/// Excel处理工具类
/// </summary>
public static class ExcelHelper
{
    /// <summary>
    /// 检查列名是否正确
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static bool ValidateHeaders(DataTable dataTable, params string[] headers)
    {
        if (dataTable.Columns.Count < headers.Length)
            return false;

        // 顺序比较相等
        for (var i = 0; i < headers.Length; i++)
        {
            if (dataTable.Columns[i].ColumnName != headers[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 读取Excel数据
    /// </summary>
    /// <param name="excelStream"></param>
    /// <param name="useHeaderRow"></param>
    /// <param name="startCell"></param>
    /// <param name="removeLastEmptyRow">从后往前移除Excel数据空行，中间存在的空行不会去除</param>
    /// <param name="throwIfEmpty">如果空表则抛出异常</param>
    /// <returns></returns>
    public static DataTable GetValues(
        Stream excelStream,
        bool useHeaderRow = true,
        string startCell = "A1",
        bool removeLastEmptyRow = true,
        bool throwIfEmpty = true
    )
    {
        var excel = excelStream.QueryAsDataTable(useHeaderRow, startCell: startCell);

        // 移除空行
        if (removeLastEmptyRow)
        {
            for (int i = excel.Rows.Count - 1; i >= 0; i--)
            {
                if (
                    excel
                        .Rows[i]
                        .ItemArray.All(field =>
                            field is DBNull
                            || field is null
                            || string.IsNullOrWhiteSpace(field.ToString())
                        )
                )
                {
                    excel.Rows.RemoveAt(i);
                }
                else
                    break;
            }
        }

        if (excel.Rows.Count == 0 && throwIfEmpty)
            throw new BusinessException("Excel无有效数据");

        return excel;
    }

    /// <summary>
    /// 读取Excel数据
    /// </summary>
    /// <param name="excelStream"></param>
    /// <param name="removeLastEmptyRow">从后往前移除Excel数据空行，中间存在的空行不会去除</param>
    /// <param name="throwIfEmpty">如果空表则抛出异常</param>
    /// <returns></returns>
    public static DataTable GetValuesByNpoi(
        Stream excelStream,
        bool removeLastEmptyRow = true,
        bool throwIfEmpty = true
    )
    {
        var workbook = WorkbookFactory.Create(excelStream);
        var sheet = workbook.GetSheetAt(0);
        if (sheet.LastRowNum <= 1 && throwIfEmpty)
            throw new BusinessException("Excel无有效数据");
        var header = sheet.GetRow(0);
        List<string> headers = new();
        foreach (var headerCell in header.Cells)
        {
            if (headerCell.ToString().IsNullOrWhiteSpace())
                break;

            headers.Add(headerCell.ToString()!);
        }

        var dataTable = new DataTable();
        headers.ForEach(it => dataTable.Columns.Add(it));

        for (int row = 1; row <= sheet.LastRowNum; row++)
        {
            List<string> values = new(headers.Count);
            var rowData = sheet.GetRow(row);
            if (rowData == null)
                values.AddRange(headers.Select(it => string.Empty));
            else
            {
                for (int col = 0; col < headers.Count; col++)
                {
                    if (col >= rowData.LastCellNum)
                        values.Add(string.Empty);
                    else
                    {
                        if (rowData.GetCell(col) == null)
                            values.Add(string.Empty);
                        else
                            values.Add(rowData.GetCell(col).ToString() ?? string.Empty);
                    }
                }
            }

            dataTable.AddRow(values.ToArray());
        }

        // 移除空行
        if (removeLastEmptyRow)
        {
            for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
            {
                if (
                    dataTable
                        .Rows[i]
                        .ItemArray.All(field =>
                            field is DBNull
                            || field is null
                            || string.IsNullOrWhiteSpace(field.ToString())
                        )
                )
                {
                    dataTable.Rows.RemoveAt(i);
                }
                else
                    break;
            }
        }

        return dataTable;
    }

    /// <summary>
    /// 检查某一列是否有数据为空
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex">从0开始</param>
    public static void ThrowIfEmptyColumn(DataTable dataTable, int columnIndex)
    {
        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(rows[i][columnIndex].ToString()))
            {
                throw new BusinessException($"第{i + 2}行{columnIndex + 1}列数据不能为空");
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不是正浮点数
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex">从0开始</param>
    /// <param name="includeZero">是否包含0</param>
    public static void ThrowIfNotPositiveDouble(
        DataTable dataTable,
        int columnIndex,
        bool includeZero = true
    )
    {
        var columnName = dataTable.Columns[columnIndex].ColumnName;

        var rows = dataTable.Rows;

        string errFormatString = "第{0}行{1}列数据无效,必须为大于{2}的有效数字";
        for (int i = 0; i < rows.Count; i++)
        {
            if (!double.TryParse(rows[i][columnIndex].ToString(), out var data))
            {
                throw new BusinessException(
                    string.Format(
                        errFormatString,
                        i + 2,
                        columnIndex + 1,
                        includeZero ? "等于0" : "0"
                    )
                );
            }

            bool condition = includeZero ? data < 0 : data <= 0;

            if (condition)
            {
                throw new BusinessException(
                    string.Format(
                        errFormatString,
                        i + 2,
                        columnIndex + 1,
                        includeZero ? "等于0" : "0"
                    )
                );
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不是正整数
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex">从0开始</param>
    /// <param name="includeZero">是否包含0</param>
    public static void ThrowIfNotPositiveInt(
        DataTable dataTable,
        int columnIndex,
        bool includeZero = true
    )
    {
        var columnName = dataTable.Columns[columnIndex].ColumnName;
        string errFormatString = "第{0}行{1}列数据无效,必须为大于{2}的有效整数";

        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!double.TryParse(rows[i][columnIndex].ToString(), out var data))
            {
                throw new BusinessException(
                    string.Format(errFormatString, i + 2, columnIndex + 1, includeZero ? "等于0" : "")
                );
            }

            bool condition = includeZero ? data < 0 : data <= 0;
            if (condition)
            {
                throw new BusinessException(
                    string.Format(
                        errFormatString,
                        i + 2,
                        columnIndex + 1,
                        includeZero ? "等于0" : "0"
                    )
                );
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不是数字
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex">从0开始</param>
    public static void ThrowIfNotDouble(DataTable dataTable, int columnIndex)
    {
        string errFormatString = "第{0}行{1}列数据无效,必须为有效数字";

        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!double.TryParse(rows[i][columnIndex].ToString(), out var data))
            {
                throw new BusinessException(string.Format(errFormatString, i + 2, columnIndex + 1));
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不是整数
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex">从0开始</param>
    public static void ThrowIfNotInt(DataTable dataTable, int columnIndex)
    {
        string errFormatString = "第{0}行{1}列数据无效,必须为有效整数数字";

        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!int.TryParse(rows[i][columnIndex].ToString(), out var data))
            {
                throw new BusinessException(string.Format(errFormatString, i + 2, columnIndex + 1));
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不在有效范围内
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex">从0开始</param>
    /// <param name="validValues">有效数据</param>
    public static void ThrowIfNotInRange(
        DataTable dataTable,
        int columnIndex,
        params string[] validValues
    )
    {
        string errFormatString = "第{0}行{1}列数据不在有效范围内,有效数据为{2}";

        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!validValues.Contains(rows[i][columnIndex].ToString() ?? string.Empty))
            {
                throw new BusinessException(
                    string.Format(
                        errFormatString,
                        i + 2,
                        columnIndex + 1,
                        string.Join(",", validValues)
                    )
                );
            }
        }
    }

    /// <summary>
    /// 判断是否有任何一列满足条件，满足则抛出异常
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex">从0开始</param>
    /// <param name="predicate"></param>
    /// <param name="errorMessage">错误信息会传递两个参数，依次为行号、列号</param>
    /// <exception cref="BusinessException"></exception>
    public static void ThrowIfAny(
        DataTable dataTable,
        int columnIndex,
        Func<string, bool> predicate,
        string errorMessage
    )
    {
        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            var hasError = predicate(rows[i][columnIndex].ToString() ?? string.Empty);
            if (hasError)
            {
                throw new BusinessException(string.Format(errorMessage, i + 2, columnIndex + 1));
            }
        }
    }

    /// <summary>
    /// 创建表格
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static DataTable CreateDatatable(params string[] columns)
    {
        var dataTable = new DataTable();
        foreach (var column in columns)
        {
            dataTable.Columns.Add(column);
        }

        return dataTable;
    }

    /// <summary>
    /// 移除excel中所有空行
    /// </summary>
    /// <param name="dataTable"></param>
    public static void RemoveAllEmptyRow(DataTable dataTable)
    {
        for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
        {
            if (
                dataTable
                    .Rows[i]
                    .ItemArray.All(field =>
                        field is DBNull
                        || field is null
                        || string.IsNullOrWhiteSpace(field.ToString())
                    )
            )
            {
                dataTable.Rows.RemoveAt(i);
            }
        }
    }
}
