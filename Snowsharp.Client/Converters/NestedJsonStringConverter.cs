using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowsharp.Client.Converters;

public class NestedJsonStringConverter<T>: JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var input = reader.GetString();
        return (string.IsNullOrEmpty(input) ? default : JsonSerializer.Deserialize<T>(input))!;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(JsonSerializer.Serialize(value!));
    }
}