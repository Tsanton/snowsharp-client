using System.Text.Json.Serialization;

namespace Snowsharp.Client.Models.Entities;

public class Schema:ISnowflakeEntity
{
    public string Name { get; set; } = default!;
    [JsonPropertyName("database_name")]
    public string DatabaseName { get; set; } = default!;
    public string Owner { get; set; } = default!;
    public string Comment { get; set; } = default!;
    [JsonPropertyName("retention_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int RetentionTime { get; set; }
    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }
}