using Snowsharp.Client.Models.Commons;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;

public class Role : ISnowflakeDescribable, ISnowflakePrincipal, ISnowflakeTaggable
{
    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; init; }

    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW ROLES LIKE '{0}';", Name).ToUpper();
    }

    public string GetObjectIdentifier()
    {
        return Name;
    }

    public string GetObjectType()
    {
        return SnowflakeObject.Role.ToSingularString();
    }

    public bool IsProcedure()
    {
        return false;
    }
}