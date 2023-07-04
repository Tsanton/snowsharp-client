using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowsharp.Client.Converters;

public class JsonSnowflakeDatetimeConverter : JsonConverter<DateTime>
{
    private readonly string[] formats = new string[]
    {
        "yyyy-MM-dd HH:mm:ss.ffffffK",
        "yyyy-MM-dd'T'HH:mm:ss.fffffffK"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string dateString = reader.GetString() ?? string.Empty;
        if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return result;
        }

        // If none of the formats match, handle the error accordingly
        throw new JsonException($"Invalid date format: {dateString}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        //"o" format represent the DateTime object in the ISO 8601 format: "2023-07-04T12:51:58.3550000Z"
        writer.WriteStringValue(value.ToString("o", CultureInfo.InvariantCulture));
    }
}