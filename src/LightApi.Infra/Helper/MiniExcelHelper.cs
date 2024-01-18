﻿using System.Data;
using LightApi.Infra.InfraException;
using MiniExcelLibs;

namespace LightApi.Infra.Helper;

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
        if (dataTable.Columns.Count < headers.Length) return false;

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
    public static DataTable GetValues(Stream excelStream, bool useHeaderRow = true,
        string startCell = "A1", bool removeLastEmptyRow = true,bool throwIfEmpty=true)
    {
        var excel = excelStream.QueryAsDataTable(useHeaderRow, startCell:startCell);

        // 移除空行
        if (removeLastEmptyRow)
        {
            for (int i = excel.Rows.Count-1; i >=0 ; i--)
            {
                if (excel.Rows[i].ItemArray.All(field => field is DBNull || field is null || string.IsNullOrWhiteSpace(field.ToString())))
                {
                    excel.Rows.RemoveAt(i);
                }
                
            }
        }

        if (excel.Rows.Count == 0&&throwIfEmpty) throw new BusinessException("Excel无有效数据");
        
        return excel;
    }
    
    
    /// <summary>
    /// 检查某一列是否有数据为空
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex"></param>
    public static void ThrowIfEmptyColumn(DataTable dataTable,int columnIndex)
    {
        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(rows[i][columnIndex].ToString()))
            {
                throw new BusinessException($"第{i+1}行{columnIndex+1}列数据不能为空");
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不是正浮点数
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex"></param>
    /// <param name="includeZero">是否包含0</param>
    public static void ThrowIfNotPositiveDouble(DataTable dataTable,int columnIndex,bool includeZero=true)
    {
        var columnName=dataTable.Columns[columnIndex].ColumnName;

        var rows = dataTable.Rows;

        string errFormatString = "第{0}行{1}列数据无效,必须为大于{2}的有效数字";
        for (int i = 0; i < rows.Count; i++)
        {
            if (!double.TryParse(rows[i][columnIndex].ToString(),out var data))
            {
                throw new BusinessException(string.Format(errFormatString,i+2,columnIndex+1,includeZero?"等于0":"0"));
            }

            bool condition=includeZero?data<0:data<=0;
            
            if (condition)
            {
                throw new BusinessException(string.Format(errFormatString,i+2,columnIndex+1,includeZero?"等于0":"0"));
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不是正整数
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex"></param>
    /// <param name="includeZero">是否包含0</param>
    public static void ThrowIfNotPositiveInt(DataTable dataTable,int columnIndex,bool includeZero=true)
    {
        var columnName=dataTable.Columns[columnIndex].ColumnName;
        string errFormatString = "第{0}行{1}列数据无效,必须为大于{2}的有效整数";

        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!double.TryParse(rows[i][columnIndex].ToString(),out var data))
            {
                throw new BusinessException(string.Format(errFormatString,i+2,columnIndex+1,includeZero?"等于0":""));
            }
            bool condition=includeZero?data<0:data<=0;
            if (condition )
            {
                throw new BusinessException(string.Format(errFormatString,i+2,columnIndex+1,includeZero?"等于0":"0"));
            }
        }
    }
    
    /// <summary>
    /// 检查某一列是否有数据不是数字
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex"></param>
    public static void ThrowIfNotDouble(DataTable dataTable,int columnIndex)
    {
        string errFormatString = "第{0}行{1}列数据无效,必须为有效数字";

        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!double.TryParse(rows[i][columnIndex].ToString(),out var data))
            {
                throw new BusinessException(string.Format(errFormatString,i+2,columnIndex+1));
            }
        }
    }
    /// <summary>
    /// 检查某一列是否有数据不是整数
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex"></param>
    public static void ThrowIfNotInt(DataTable dataTable,int columnIndex)
    {
        string errFormatString = "第{0}行{1}列数据无效,必须为有效整数数字";

        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!int.TryParse(rows[i][columnIndex].ToString(),out var data))
            {
                throw new BusinessException(string.Format(errFormatString,i+2,columnIndex+1));
            }
        }
    }

    /// <summary>
    /// 检查某一列是否有数据不在有效范围内
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnIndex"></param>
    /// <param name="validValues">有效数据</param>
    public static void ThrowIfNotInRange(DataTable dataTable,int columnIndex,params string[] validValues)
    {
        string errFormatString = "第{0}行{1}列数据不在有效范围内";
        
        var rows = dataTable.Rows;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!validValues.Contains(rows[i][columnIndex].ToString()??string.Empty))
            {
                throw new BusinessException(string.Format(errFormatString,i+2,columnIndex+1));
            }
        }
    }
}