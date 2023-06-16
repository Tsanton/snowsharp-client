using Snowflake.Client.Model;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets.Grants;

public class SchemaGrant : ISnowflakeGrantAsset
{
    public SchemaGrant(string databaseName, string schemaName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
    }

    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string GetGrantStatement(ISnowflakePrincipal principal, List<Privilege> privileges)
    {
        var flatPrivileges = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (principal)
        {
            case Role:
                return string.Format("GRANT {0} ON SCHEMA {1}.{2} TO ROLE {3};", 
                    flatPrivileges, 
                    DatabaseName, 
                    SchemaName,
                    principal.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("GRANT {0} ON SCHEMA {1}.{2} TO DATABASE ROLE {3};", 
                    flatPrivileges, 
                    DatabaseName, 
                    SchemaName,
                    principal.GetIdentifier()
                );
            default:
                throw new NotImplementedException("GetGrantStatement is not implemented for this interface type");
        }
    }

    public string GetRevokeStatement(ISnowflakePrincipal principal, List<Privilege> privileges)
    {
        var flatPrivileges = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (principal)
        {
            case Role:
                return string.Format("REVOKE {0} ON SCHEMA {1}.{2} FROM ROLE {3} CASCADE;",
                    flatPrivileges,
                    DatabaseName, 
                    SchemaName,
                    principal.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("REVOKE {0} ON SCHEMA {1}.{2} FROM DATABASE ROLE {3} CASCADE;", 
                    flatPrivileges, 
                    DatabaseName, 
                    SchemaName,
                    principal.GetIdentifier()
                );
            default:
                throw new NotImplementedException("GetRevokeStatement is not implemented for this interface type");
        }
    }

    public bool ValidateGrants(List<Privilege> privileges)
    {
        throw new NotImplementedException();
    }
}