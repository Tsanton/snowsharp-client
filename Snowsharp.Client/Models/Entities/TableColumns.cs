using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;

namespace Snowsharp.Client.Models.Entities;

public class ColumnType
{
    public string Type { get; set; } = default!;
    public bool Nullable { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public int? Length { get; set; }
    public bool? Fixed { get; set; }
}


public class Column
{
    public string Name { get; set; } = default!;
    [JsonPropertyName("data_type")]
    public ColumnType ColumnType { get; set; } = default!;
    public string? Default { get; set; }
    public string? Check { get; set; }
    public string? Expression { get; set; }
    [JsonPropertyName("primary key")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool PrimaryKey { get; set; }
    [JsonPropertyName("unique key")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool UniqueKey { get; set; }
    [JsonPropertyName("policy name")]
    public string? PolicyName { get; set; }
    [JsonPropertyName("auto_increment")]
    public string? AutoIncrement { get; set; }
    public List<ClassificationTag> Tags { get; init; } = default!;
    public string Comment { get; set; } = default!;
}