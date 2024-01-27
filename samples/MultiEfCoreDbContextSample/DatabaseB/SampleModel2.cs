using LightApi.EFCore.Entities;

namespace MultiEfCoreDbContextSample.Database;

public class SampleModel2:EfEntity<int>
{
    public string Name { get; set; }
    
    public int Age { get; set; }
}