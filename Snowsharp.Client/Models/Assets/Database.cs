using Snowsharp.Client.Models.Commons;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

public class Database : ISnowflakeAsset, ISnowflakeTaggable
{
    public Database(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
    public string Comment { get; init; } = "SNOWPLOW TEST DATABASE";
    public ISnowflakePrincipal Owner { get; init; } = new Role("USERADMIN");

    public string GetCreateStatement()
    {
        SnowflakePrincipal ownerType;
        switch (Owner)
        {
            case Role:
                ownerType = SnowflakePrincipal.Role;
                break;
            default:
                throw new NotImplementedException("Ownership is not implementer for this interface type");
        }

        return string.Format(@"
CREATE OR REPLACE DATABASE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON DATABASE {0} TO {2} {3};",
            Name, Comment, ownerType.GetSnowflakeType(), Owner.GetObjectIdentifier()
        );
    }

    public string GetDeleteStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format(@"
DROP DATABASE IF EXISTS {0} CASCADE;
", Name);
    }

    public string GetObjectIdentifier()
    {
        return Name;
    }

    public string GetObjectType()
    {
        return "DATABASE";
    }
}