using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;

namespace Snowsharp.Client.Models.Entities;

public class Tag: ISnowflakeEntity
{
    [JsonPropertyName("database_name")]
    public string DatabaseName { get; set; } = default!;
    [JsonPropertyName("schema_name")]
    public string SchemaName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Owner { get; set; } = default!;
    public string Comment { get; set; } = default!;
    [JsonPropertyName("allowed_values")]
    [JsonConverter(typeof(NestedJsonStringConverter<List<string>>))]
    public List<string> AllowedValues { get; set; } = default!;
    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }
}