using Snowsharp.Client.Models.Commons;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets.Grants;

public class DatabaseGrant : ISnowflakeGrantAsset
{
    public DatabaseGrant(string databaseName)
    {
        DatabaseName = databaseName;
    }

    public string DatabaseName { get; init; }
    public string GetGrantStatement(ISnowflakePrincipal principal, List<Privilege> privileges)
    {
        var flatPrivileges = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        return principal.GetObjectType() switch
        {
            "ROLE" => string.Format("GRANT {0} ON DATABASE {1} TO ROLE {2};", flatPrivileges, DatabaseName, principal.GetObjectIdentifier()),
            "DATABASE_ROLE" => string.Format("GRANT {0} ON DATABASE {1} TO DATABASE ROLE {2};", flatPrivileges, DatabaseName, principal.GetObjectIdentifier()),
            _ => throw new NotImplementedException("GetGrantStatement is not implemented for this interface type"),
        };
    }

    public string GetRevokeStatement(ISnowflakePrincipal principal, List<Privilege> privileges)
    {
        var flatPrivileges = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        return principal.GetObjectType() switch
        {
            "ROLE" => string.Format("REVOKE {0} ON DATABASE {1} FROM ROLE {2} CASCADE;", flatPrivileges, DatabaseName, principal.GetObjectIdentifier()),
            "DATABASE_ROLE" => string.Format("REVOKE {0} ON DATABASE {1} FROM DATABASE ROLE {2} CASCADE;", flatPrivileges, DatabaseName, principal.GetObjectIdentifier()),
            _ => throw new NotImplementedException("GetRevokeStatement is not implemented for this interface type"),
        };
    }

    public bool ValidateGrants(List<Privilege> privileges)
    {
        throw new NotImplementedException();
    }
}