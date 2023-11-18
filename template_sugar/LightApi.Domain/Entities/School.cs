using LightApi.SqlSugar;
using SqlSugar;

namespace LightApi.Domain;

public class School:ISugarTable
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    [Navigate(typeof(StudentSchoolMapping), nameof(StudentSchoolMapping.SchoolId), nameof(StudentSchoolMapping.StudentId))]//注意顺序
    public List<Student>? Students { get; set; }//只能是null不能赋默认值
}