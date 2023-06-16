using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Entities;

public class FutureGrant: ISnowflakeEntity
{
    [JsonPropertyName("grantee_name")]
    public string RoleName { get; init; } = default!;
    [JsonPropertyName("grant_to")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("grant_on")]
    public string GrantOn { get; init; } = default!; //TODO: Enum
    [JsonPropertyName("name")]
    public string GrantTargetName { get; init; } = default!;
    public Privilege Privilege { get; init; }
    [JsonPropertyName("grant_option")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool GrantOption { get; init; }
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
}
