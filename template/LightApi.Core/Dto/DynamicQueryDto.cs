using System.Text.Json.Serialization;
using LightApi.Infra.Extension.DynamicQuery;

namespace LightApi.Core.Dto;

public class DynamicQueryDto
{
    public string Property { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DynamicOpType OpType { get; set; }
    public object Value { get; set; }
}