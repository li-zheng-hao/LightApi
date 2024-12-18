using Dapper;

namespace LightApi.EFCore.Dapper;

public static class SqlBuilderExtension
{
    /// <summary>
    /// 条件拼接
    /// </summary>
    /// <param name="sqlBuilder"></param>
    /// <param name="condition"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static SqlBuilder WhereIf(
        this SqlBuilder sqlBuilder,
        bool? condition,
        string sql,
        dynamic? parameters = null
    )
    {
        if (condition == true)
        {
            return sqlBuilder.Where(sql, parameters);
        }

        return sqlBuilder;
    }

    /// <summary>
    /// 条件拼接
    /// </summary>
    /// <param name="sqlBuilder"></param>
    /// <param name="condition"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static SqlBuilder OrderByIf(
        this SqlBuilder sqlBuilder,
        bool? condition,
        string sql,
        dynamic? parameters = null
    )
    {
        if (condition == true)
        {
            return sqlBuilder.OrderBy(sql, parameters);
        }

        return sqlBuilder;
    }
}
