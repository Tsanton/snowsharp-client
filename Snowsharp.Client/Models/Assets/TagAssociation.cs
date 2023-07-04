using Snowsharp.Client.Models.Commons;

namespace Snowsharp.Client.Models.Assets;

public class TagAssociation : ISnowflakeAsset
{
    public TagAssociation(ISnowflakeTaggable taggable, Tag tag, string value)
    {
        Taggable = taggable;
        Tag = tag;
        Value = value;
    }

    public ISnowflakeTaggable Taggable { get; }
    public Tag Tag { get; }
    public string Value { get; set; }

    // public string GetDescribeStatement()
    // {

    //     return $"SELECT * from table(SNOWFLAKE.INFORMATION_SCHEMA.TAG_REFERENCES('{Taggable.GetObjectIdentifier()}', '{objectType}'));";
    // }

    public string GetCreateStatement()
    {
        string objectType = Taggable.GetObjectType() switch
        {
            "DATABASE" => "DATABASE",
            "SCHEMA" => "SCHEMA",
            "TABLE" or "VIEW" or "MATERIALIZED VIEW" => "TABLE",
            "ROLE" => "ROLE",
            _ => throw new ArgumentException("Invalid object type"),
        };
        //ALTER DATABASE <name> SET TAG <tag_name> = '<tag_value>': https://docs.snowflake.com/en/sql-reference/sql/alter-database
        //ALTER SCHEMA <name> SET TAG <tag_name> = '<tag_value>': https://docs.snowflake.com/en/sql-reference/sql/alter-schema
        //ALTER TABLE <name> SET TAG <tag_name> = '<tag_value>': https://docs.snowflake.com/en/sql-reference/sql/alter-table
        //ALTER ROLE <name> SET TAG <tag_name> = '<tag_value>' : https://docs.snowflake.com/en/sql-reference/sql/alter-role
        return $"ALTER {objectType} {Taggable.GetObjectIdentifier()} SET TAG {Tag.DatabaseName}.{Tag.SchemaName}.{Tag.TagName} = '{Value}';";
    }

    public string GetDeleteStatement()
    {
        string objectType = Taggable.GetObjectType() switch
        {
            "DATABASE" => "DATABASE",
            "SCHEMA" => "SCHEMA",
            "TABLE" or "VIEW" or "MATERIALIZED VIEW" => "TABLE",
            "ROLE" => "ROLE",
            _ => throw new ArgumentException("Invalid object type"),
        };
        return $"ALTER {objectType} {Taggable.GetObjectIdentifier()} UNSET TAG {Tag.DatabaseName}.{Tag.SchemaName}.{Tag.TagName};";
    }
}