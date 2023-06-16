using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowsharp.Client.Converters;

public class JsonSnowflakeDatetimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss.ffffffK";
    //System.FormatException: String '2023-03-02 16:17:12.291000+01:00' was not recognized as a valid DateTime.

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString() ?? string.Empty, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}