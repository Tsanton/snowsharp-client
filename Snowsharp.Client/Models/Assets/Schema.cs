using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

public class Schema:ISnowflakeAsset
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
        SnowflakePrincipal ownerType;
        switch (Owner)
        {
            case Role principal:
                ownerType = SnowflakePrincipal.Role;
                break;
            case DatabaseRole principal:
                ownerType = SnowflakePrincipal.DatabaseRole;
                break;
            default:
                throw new NotImplementedException("Ownership is not implementer for this interface type");
        }
        return string.Format(@"
CREATE OR REPLACE SCHEMA {0}.{1} WITH MANAGED ACCESS COMMENT = '{2}';
GRANT OWNERSHIP ON SCHEMA {0}.{1} TO {3} {4} REVOKE CURRENT GRANTS;", 
            DatabaseName, Name, Comment, ownerType.GetSnowflakeType(), Owner.GetIdentifier()
        );
    }

    public string GetDeleteStatement()
    {
        return string.Format(@"DROP SCHEMA IF EXISTS {0}.{1} CASCADE;", 
            DatabaseName, Name
        );
    }
}