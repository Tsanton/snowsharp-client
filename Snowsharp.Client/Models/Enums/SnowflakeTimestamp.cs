using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;

namespace Snowsharp.Client.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<SnowflakeTimestamp>))]
public enum SnowflakeTimestamp
{
    [JsonPropertyName("TIMESTAMP_LTZ")] LocalTimeZone, //TIMESTAMP_LTZ internally stores UTC time with a specified precision. However, all operations are performed in the current session’s time zone
    [JsonPropertyName("TIMESTAMP_NTZ")] WithoutTimeZone, //TIMESTAMP_NTZ internally stores “wallclock” time with a specified precision. All operations are performed without taking any time zone into account
    [JsonPropertyName("TIMESTAMP_TZ")] WithTimeZone, //TIMESTAMP_TZ internally stores UTC time together with an associated time zone offset
}