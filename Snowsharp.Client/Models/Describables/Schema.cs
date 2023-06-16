namespace Snowsharp.Client.Models.Describables;

public class Schema:ISnowflakeDescribable
{
    public Schema(string databaseName, string schemaName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
    }

    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW SCHEMAS LIKE '{0}' IN DATABASE {1};", SchemaName, DatabaseName).ToUpper();
    }
}