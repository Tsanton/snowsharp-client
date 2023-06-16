using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Entities;

public class Grant: ISnowflakeEntity
{
    [JsonPropertyName("grantee_name")]
    public string GranteeIdentifier { get; init; } = default!;
    [JsonPropertyName("granted_to")]
    public SnowflakePrincipal PrincipalType { get; init; } //TODO:Not correct for DATABASE_ROLE in PrincipalDescendants
    [JsonPropertyName("granted_on")]
    public string GrantedOn { get; init; } = default!; //TODO: Enum
    [JsonPropertyName("name")]
    public string GrantedIdentifier { get; init; } = default!;
    public Privilege Privilege { get; init; }
    [JsonPropertyName("grant_option")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool GrantOption { get; init; }
    [JsonPropertyName("granted_by")]
    public string GrantedBy { get; init; } = default!;
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
    [JsonPropertyName("distance_from_source")]
    public int DistanceFromSource { get; init; } = 0;
}
