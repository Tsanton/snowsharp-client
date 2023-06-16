namespace Snowsharp.Client.Models.Describables;

public class Tag: ISnowflakeDescribable
{
    public Tag(string databaseName, string schemaName, string tagName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
        TagName = tagName;
    }

    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string TagName { get; init; }
    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW TAGS LIKE '{0}' IN SCHEMA {1}.{2};", TagName, DatabaseName, SchemaName).ToUpper();
    }
}