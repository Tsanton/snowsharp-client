using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowsharp.Client.Converters;

public class JsonStringEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, System.Enum
{

    private readonly Dictionary<TEnum, string> _enumToString = new();
    private readonly Dictionary<string, TEnum> _stringToEnum = new();

    public JsonStringEnumConverter()
    {
        var type = typeof(TEnum);
        var values = Enum.GetValues<TEnum>();

        foreach (var value in values)
        {
            var enumMember = type.GetMember(value.ToString())[0];
            var attr = enumMember.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                .Cast<JsonPropertyNameAttribute>()
                .FirstOrDefault();

            _stringToEnum.Add(value.ToString(), value);

            if (attr?.Name != null)
            {
                _enumToString.Add(value, attr.Name);
                _stringToEnum.Add(attr.Name, value);
            } else
            {
                _enumToString.Add(value, value.ToString());
            }
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue == "") { return default; }

        return stringValue != null && _stringToEnum.TryGetValue(stringValue, out var enumValue) ? enumValue : throw new Exception("Error converting value to enum");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(_enumToString[value]);
    }
}
