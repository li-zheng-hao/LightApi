using System.Reflection;
using LightApi.EFCore.Config;

namespace LightApi.Domain;

public class EntityInfo:AbstractSharedEntityInfo
{
    protected override Assembly GetCurrentAssembly() => GetType().Assembly;
    
}