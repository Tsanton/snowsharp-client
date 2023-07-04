using System.Text.Json.Serialization;

namespace Snowsharp.Client.Models.Entities;

public class TagAssociation : ISnowflakeEntity
{
    [JsonPropertyName("tag_database")]
    public string TagDatabase { get; set; } = null!;

    [JsonPropertyName("tag_schema")]
    public string TagSchema { get; set; } = null!;

    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = null!;

    [JsonPropertyName("tag_value")]
    public string? TagValue { get; set; }

    [JsonPropertyName("level")]
    public string Level { get; set; } = null!;

    [JsonPropertyName("object_database")]
    public string ObjectDatabase { get; set; } = null!;

    [JsonPropertyName("object_schema")]
    public string ObjectSchema { get; set; } = null!;

    [JsonPropertyName("object_name")]
    public string ObjectName { get; set; } = null!;

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = null!;

    [JsonPropertyName("column_name")]
    public string? ColumnName { get; set; }
}