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
        string objectType;
        switch (Taggable.GetObjectType())
        {
            case "DATABASE":
                objectType = "DATABASE";
                break;
            case "SCHEMA":
                objectType = "DATABASE";
                break;
            case "TABLE":
            case "VIEW":
            case "MATERIALIZED VIEW":
                objectType = "TABLE";
                break;
            case "ROLE":
                objectType = "ROLE";
                break;
            default:
                throw new ArgumentException("Invalid object type");
        }

        return $"SELECT * from table(SNOWFLAKE.INFORMATION_SCHEMA.TAG_REFERENCES('{Taggable.GetObjectIdentifier()}', '{objectType}'));";
    }

    public bool IsProcedure()
    {
        return true;
    }
}