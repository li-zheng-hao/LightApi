using System.Reflection;
using LightApi.EFCore.Config;

namespace MultiEfCoreDbContextSample.Database;

public class EntityInfo2:AbstractSharedEntityInfo
{
    protected override Assembly GetCurrentAssembly()
    {
        return this.GetType().Assembly;
    }

    protected override IEnumerable<Type> GetEntityTypes(Assembly assembly)
    {
        return new[] { typeof(SampleModel2) };
    }
}