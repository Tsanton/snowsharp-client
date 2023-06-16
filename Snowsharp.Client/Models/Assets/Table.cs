using System.Text;

namespace Snowsharp.Client.Models.Assets;

public class Table:ISnowflakeAsset
{
    public Table(string databaseName, string schemaName, string tableName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
        TableName = tableName;
    }

    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string TableName { get; init; }
    // public string? TableType { get; init; } //[ { [ LOCAL | GLOBAL ] TEMPORARY | VOLATILE } | TRANSIENT ]
    public List<SnowflakeColumn> Columns { get; init; } = new();

    public List<ClassificationTag> Tags { get; init; } = new();
    // public RowAccessPolicy RowAccessPolicy { get; init; }
    public int DataRetentionTimeInDays { get; init; } = 1;
    public string Comment { get; init; } = "SNOWPLOW TEST TABLE";
    
    public string GetCreateStatement()
    {
        var sb = new StringBuilder();
        var columnDefinitions = new List<string>();
        var primaryKeys = new List<string>();
        sb.AppendLine($"CREATE TABLE {DatabaseName}.{SchemaName}.{TableName} (");
        foreach (var c in Columns!)
        {
            columnDefinitions.Add(c.GetDefinition());
            if (c.PrimaryKey) primaryKeys.Add(c.Name);
        }
        if (primaryKeys.Count > 0) sb.Append(string.Join(",\n", columnDefinitions)); else sb.AppendLine(string.Join(",\n", columnDefinitions));
        if (primaryKeys.Count > 0) sb.AppendLine(",").AppendLine($"PRIMARY KEY({string.Join(", ", primaryKeys)})");
        sb.AppendLine(");");
        foreach (var tag in Tags)
        {
            var val = tag.TagValue ?? "";
            sb.AppendLine($"ALTER TABLE {DatabaseName}.{SchemaName}.{TableName} SET TAG {tag.GetIdentifier()} = '{val}';");
        }

        foreach (var column in Columns)
        {
            if (column.Tags.Count <= 0) continue;
            foreach (var columnTag in column.Tags)
            {
                sb.Append(' ').Append($"ALTER TABLE {DatabaseName}.{SchemaName}.{TableName} ALTER COLUMN {column.Name}");
                sb.Append(' ').AppendLine($"SET TAG {columnTag.GetIdentifier()} = '{columnTag.TagValue}';");
            }
        }
        return sb.ToString();
    }

    public string GetDeleteStatement()
    {
        return $"DROP TABLE {DatabaseName}.{SchemaName}.{TableName}";
    }
}