using System.Text.Json.Serialization;

namespace Snowsharp.Client.Models.Enums;

public static class EnumExtensions
{
    public static string GetEnumJsonAttributeValue(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field != null && Attribute.GetCustomAttribute(field, typeof(JsonPropertyNameAttribute)) is JsonPropertyNameAttribute attribute)
        {
            return attribute.Name;
        }
        throw new ArgumentException($"Attribute {nameof(enumValue)} not found.");
    }
}