using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;

namespace Snowsharp.Client.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<SnowflakePrincipal>))]
public enum SnowflakePrincipal
{
    [JsonPropertyName("ROLE")] Role,
    [JsonPropertyName("DATABASE_ROLE")] DatabaseRole,
    [JsonPropertyName("USER")] User,
}

public static class SnowflakePrincipalExtensions
{
    public static string GetSnowflakeType(this SnowflakePrincipal enumValue)
    {
        return enumValue switch
        {
            SnowflakePrincipal.Role => "ROLE",
            SnowflakePrincipal.DatabaseRole => "DATABASE ROLE",
            SnowflakePrincipal.User => "USER",
            _ => throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, "Enum value not mapped")
        };
    }
}