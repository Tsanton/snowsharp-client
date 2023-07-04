using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;

public class Database : ISnowflakeDescribable, ISnowflakeTaggable
{
    public Database(string name)
    {
        Name = name;
    }

    public string Name { get; init; }

    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW DATABASES LIKE '{0}';", Name).ToUpper();
    }

    public bool IsProcedure()
    {
        return false;
    }

    public string GetObjectType()
    {
        return SnowflakeObject.Table.ToSingularString();
    }

    public string GetObjectIdentifier()
    {
        return Name;
    }
}