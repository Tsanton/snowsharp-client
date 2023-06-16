using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Entities;

public class RoleInheritance: ISnowflakeEntity
{
    [JsonPropertyName("grantee_name")]
    public string PrincipalIdentifier { get; init; } = default!;
    [JsonPropertyName("granted_to")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("name")]
    public string InheritedRoleIdentifier { get; init; } = default!;
    [JsonPropertyName("granted_on")]
    public SnowflakePrincipal InheritedRoleType { get; init; }
    public Privilege Privilege { get; init; }
    [JsonPropertyName("grant_option")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool GrantOption { get; init; }
    [JsonPropertyName("granted_by")]
    public string GrantedBy { get; init; } = default!;
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
}