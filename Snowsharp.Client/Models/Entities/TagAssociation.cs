using System.Text.Json.Serialization;

namespace Snowsharp.Client.Models.Entities;

public class TagAssociation: ISnowflakeEntity
{
    [JsonPropertyName("TagDatabase")]
    public string TagDatabase { get; set; } = null!;

    [JsonPropertyName("TagSchema")]
    public string TagSchema { get; set; } = null!;

    [JsonPropertyName("TagName")]
    public string TagName { get; set; } = null!;

    [JsonPropertyName("TagValue")]
    public string? TagValue { get; set; }

    [JsonPropertyName("Level")]
    public string Level { get; set; } = null!;

    [JsonPropertyName("ObjectDatabase")]
    public string ObjectDatabase { get; set; } = null!;

    [JsonPropertyName("ObjectSchema")]
    public string ObjectSchema { get; set; } = null!;

    [JsonPropertyName("ObjectName")]
    public string ObjectName { get; set; } = null!;

    [JsonPropertyName("Domain")]
    public string Domain { get; set; } = null!;

    [JsonPropertyName("ColumnName")]
    public string? ColumnName { get; set; }
}