

using Newtonsoft.Json;

namespace LightApi.Core.Converter;

/// <summary>
/// double序列化自动保留两位小数
/// </summary>
public class DecimalTwoDigitalConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(decimal).IsAssignableFrom(objectType) || typeof(decimal?).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        decimal value = reader.Value == null ? 0 : Convert.ToDecimal(reader.Value);
        value = Math.Round(value, 2, MidpointRounding.AwayFromZero);
        return value;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var formattedValue = $"{value:0.00}";
        writer.WriteRawValue(formattedValue);
    }
}