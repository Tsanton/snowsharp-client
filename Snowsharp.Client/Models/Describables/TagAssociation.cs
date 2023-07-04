using Snowsharp.Client.Models.Commons;

namespace Snowsharp.Client.Models.Describables;

public class TagAssociation : ISnowflakeDescribable
{
    public TagAssociation(ISnowflakeTaggable taggable)
    {
        Taggable = taggable;
    }

    public ISnowflakeTaggable Taggable { get; init; }

    public string GetDescribeStatement()
    {
        string objectType = Taggable.GetObjectType() switch
        {
            "DATABASE" => "DATABASE",
            "SCHEMA" => "SCHEMA",
            "TABLE" or "VIEW" or "MATERIALIZED VIEW" => "TABLE",
            "ROLE" => "ROLE",
            _ => throw new ArgumentException("Invalid object type"),
        };
        return $"SELECT * from table(SNOWFLAKE.INFORMATION_SCHEMA.TAG_REFERENCES('{Taggable.GetObjectIdentifier()}', '{objectType}'));";
    }

    public bool IsProcedure()
    {
        return false;
    }
}