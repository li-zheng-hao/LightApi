using LightApi.EFCore.Entities;

namespace LightApi.Domain.Entities;

[NonAutoIncrement]
public class School:EfEntity<long>
{
    public string Name { get; set; }    
}

[NonAutoIncrement]
public class Student:EfEntity<long>,ISoftDeleteV2
{
    public string Name { get; set; }
    
    public long? SchoolId { get; set; }
    
    public School? School { get; set; }
    
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}