using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LightApi.EFCore.Entities;

public interface IEfEntity
{
    
}

public interface IEfEntity<T> : IEfEntity 
{
    [JsonPropertyOrder(-10)]
    [JsonProperty(Order = -10)]
    public T Id { get; set; }
}