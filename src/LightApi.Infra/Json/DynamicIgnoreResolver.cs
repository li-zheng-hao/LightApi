using Masuit.Tools;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LightApi.Infra.Json;

public class DynamicContractResolver : DefaultContractResolver
{
    private Type[] _typeToIgnore;

    /// <summary>
    /// Newtonsoft序列化时忽略指定类型
    /// </summary>
    public DynamicContractResolver(params Type[] ignoreTypes)
    {
        if (ignoreTypes.IsNullOrEmpty())
            _typeToIgnore = new Type[]
            {
                typeof(IFormFile),
                typeof(IFormFileCollection),
                typeof(byte[]),
                typeof(Stream)
            };
        else
        {
            _typeToIgnore = ignoreTypes;
        }
    }

    protected override IList<JsonProperty> CreateProperties(
        Type type,
        MemberSerialization memberSerialization
    )
    {
        IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

        properties = properties
            .Where(p => !_typeToIgnore.Any(t => t.IsAssignableFrom(p.PropertyType)))
            .ToList();

        return properties;
    }
}
