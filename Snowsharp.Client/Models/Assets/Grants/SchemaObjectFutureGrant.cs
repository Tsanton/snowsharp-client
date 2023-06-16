using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets.Grants;

public class SchemaObjectFutureGrant : ISnowflakeGrantAsset
{
    public SchemaObjectFutureGrant(string databaseName, string schemaName, SnowflakeObject grantTarget)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
        GrantTarget = grantTarget;
    }

    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public SnowflakeObject GrantTarget { get; init; }
    public string GetGrantStatement(ISnowflakePrincipal principal, List<Privilege> privileges)
    {
        var flatPrivileges = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (principal)
        {
            case Role:
                return string.Format("GRANT {0} ON FUTURE {1} IN SCHEMA {2}.{3} TO ROLE {4};", 
                    flatPrivileges, 
                    GrantTarget.ToPluralString(), 
                    DatabaseName,
                    SchemaName,
                    principal.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("GRANT {0} ON FUTURE {1} IN SCHEMA {2}.{3}  TO DATABASE ROLE {4};", 
                    flatPrivileges,
                    GrantTarget.ToPluralString(),
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
                return string.Format("REVOKE {0} ON FUTURE {1} IN SCHEMA {2}.{3} FROM ROLE {4};", 
                    flatPrivileges, 
                    GrantTarget.ToPluralString(), 
                    DatabaseName,
                    SchemaName,
                    principal.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("REVOKE {0} ON FUTURE {1} IN SCHEMA {2}.{3} FROM DATABASE ROLE {4};", 
                    flatPrivileges,
                    GrantTarget.ToPluralString(),
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