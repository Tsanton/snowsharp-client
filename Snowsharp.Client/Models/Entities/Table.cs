using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;

namespace Snowsharp.Client.Models.Entities;

public class ClassificationTag
{
    [JsonPropertyName("tag_database")]
    public string TagDatabaseName{ get; init; }  = default!;
    [JsonPropertyName("tag_schema")]
    public string TagSchemaName{ get; init; }  = default!;
    [JsonPropertyName("tag_name")]
    public string TagName{ get; init; }  = default!;
    /// Either TABLE or COLUMN: Indicates if the tag is applied directly to the column or if it is inherited from the table
    [JsonPropertyName("domain")]
    public string DomainLevel{ get; init; }  = default!;
    [JsonPropertyName("tag_value")]
    [JsonConverter(typeof(NullableEmptyStringConverter))]
    public string? TagValue{ get; init; }
    
}
public class Table: ISnowflakeEntity
{   
    [JsonPropertyName("database_name")]
    public string DatabaseName { get; init; } = default!;
    [JsonPropertyName("schema_name")]
    public string SchemaName { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Kind { get; init; } = default!;
    public string Comment { get; init; } = default!;
    [JsonPropertyName("change_tracking")]
    public string ChangeTracking { get; init; } = default!;
    [JsonPropertyName("automatic_clustering")]
    public string AutoClustering { get; init; } = default!;
    public int Rows { get; init; }
    public string Owner { get; init; } = default!;
    [JsonPropertyName("retention_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int RetentionTime { get; init; }
    public List<ClassificationTag> Tags { get; init; } = default!;
    public List<Column> Columns { get; init; } = default!;
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; set; }
}
