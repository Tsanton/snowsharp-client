using Snowsharp.Client.Models.Commons;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

public class Schema : ISnowflakeAsset, ISnowflakeTaggable
{
    public Schema(string databaseName, string name)
    {
        DatabaseName = databaseName;
        Name = name;
    }

    public string DatabaseName { get; init; }
    public string Name { get; init; }
    public string Comment { get; init; } = "SNOWPLOW TEST SCHEMA";
    public ISnowflakePrincipal Owner { get; init; } = new Role("USERADMIN");
    public string GetCreateStatement()
    {
        var ownerType = Owner switch
        {
            Role => SnowflakePrincipal.Role,
            DatabaseRole => SnowflakePrincipal.DatabaseRole,
            _ => throw new NotImplementedException("Ownership is not implementer for this interface type"),
        };
        return string.Format(@"
CREATE OR REPLACE SCHEMA {0}.{1} WITH MANAGED ACCESS COMMENT = '{2}';
GRANT OWNERSHIP ON SCHEMA {0}.{1} TO {3} {4} REVOKE CURRENT GRANTS;",
            DatabaseName, Name, Comment, ownerType.GetSnowflakeType(), Owner.GetObjectIdentifier()
        );
    }

    public string GetDeleteStatement()
    {
        return string.Format(@"DROP SCHEMA IF EXISTS {0}.{1} CASCADE;",
            DatabaseName, Name
        );
    }

    public string GetObjectType()
    {
        return "SCHEMA";
    }

    public string GetObjectIdentifier()
    {
        return $"{DatabaseName}.{Name}";
    }
}