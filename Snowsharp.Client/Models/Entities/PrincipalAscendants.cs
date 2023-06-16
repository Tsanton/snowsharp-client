using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Entities;

public class PrincipalAscendant
{
    [JsonPropertyName("grantee_name")]
    public string GranteeIdentifier { get; init; } = default!;
    [JsonPropertyName("granted_to")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("role")]
    public string GrantedIdentifier { get; init; } = default!;
    [JsonPropertyName("granted_on")]
    public SnowflakePrincipal GrantedOn { get; init; }
    [JsonPropertyName("granted_by")]
    public string GrantedBy { get; init; } = default!;
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
    [JsonPropertyName("distance_from_source")]
    public int DistanceFromSource { get; init; } = 0;
}


public class PrincipalAscendants:ISnowflakeEntity
{
    [JsonPropertyName("principal_identifier")]
    public string PrincipalIdentifier { get; init; } = default!;
    [JsonPropertyName("principal_type")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("ascendants")]
    public List<PrincipalAscendant> Ascendants { get; init; } = default!;
}
