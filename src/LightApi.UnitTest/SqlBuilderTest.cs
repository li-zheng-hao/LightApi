using Dapper;
using LightApi.EFCore.Dapper;

namespace LightApi.UnitTest;

public class SqlBuilderTest
{
    [Fact]
    public void TestOrderByIf_True()
    {
        var sqlBuilder = new SqlBuilder();
        sqlBuilder.OrderByIf(true, "order by id desc");
        var template = sqlBuilder.AddTemplate("SELECT * FROM TEST /**where**/ /**orderby**/");
        var sql = template.RawSql;
        Assert.Equal("SELECT * FROM TEST  ORDER BY order by id desc\n", sql);
    }

    [Fact]
    public void TestOrderByIf_False()
    {
        var sqlBuilder = new SqlBuilder();
        sqlBuilder.OrderByIf(false, "order by id desc");
        var template = sqlBuilder.AddTemplate("SELECT * FROM TEST /**where**/ /**orderby**/");
        var sql = template.RawSql;
        Assert.Equal("SELECT * FROM TEST  ", sql);
    }

    [Fact]
    public void TestWhereIf_False()
    {
        var sqlBuilder = new SqlBuilder();
        sqlBuilder.WhereIf(false, "1=1");
        var template = sqlBuilder.AddTemplate("SELECT * FROM TEST /**where**/ /**orderby**/");
        var sql = template.RawSql;
        Assert.Equal("SELECT * FROM TEST  ", sql);
    }
}
