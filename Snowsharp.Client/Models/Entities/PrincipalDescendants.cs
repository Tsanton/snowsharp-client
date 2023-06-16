using System.Text.Json.Serialization;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Entities;

public class PrincipalDescendants:ISnowflakeEntity
{
    [JsonPropertyName("principal_identifier")]
    public string PrincipalIdentifier { get; init; } = default!;
    [JsonPropertyName("principal_type")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("descendants")]
    public List<Grant> Descendants { get; init; } = default!;
}
