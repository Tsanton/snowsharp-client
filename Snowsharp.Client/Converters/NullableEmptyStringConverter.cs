using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowsharp.Client.Converters;

public class NullableEmptyStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var val = reader.GetString();
        return string.IsNullOrEmpty(val) ? null : val;
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}