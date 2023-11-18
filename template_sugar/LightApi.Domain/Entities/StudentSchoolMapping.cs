using LightApi.SqlSugar;
using SqlSugar;

namespace LightApi.Domain;

public class StudentSchoolMapping:ISugarTable
{
    [SugarColumn(IsPrimaryKey = true)]//中间表可以不是主键
    public int SchoolId { get; set; }
    [SugarColumn(IsPrimaryKey = true)]//中间表可以不是主键
    public int StudentId { get; set; }
}