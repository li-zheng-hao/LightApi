using LightApi.SqlSugar;
using SqlSugar;

namespace LightApi.Domain;

public class Student:ISugarTable
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    [Navigate(typeof(StudentSchoolMapping), nameof(StudentSchoolMapping.StudentId), nameof(StudentSchoolMapping.SchoolId))]//注意顺序
    public List<School>? Schools { get; set; }//只能是null不能赋默认值
}

