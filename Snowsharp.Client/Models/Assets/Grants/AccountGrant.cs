using Snowflake.Client.Model;
using Snowsharp.Client.Models.Enums;

// ReSharper disable UseStringInterpolation

namespace Snowsharp.Client.Models.Assets.Grants;

public class AccountGrant : ISnowflakeGrantAsset 
{
    public string GetGrantStatement(ISnowflakePrincipal principal, List<Privilege> privileges)
    {
        var flatPrivileges = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        return principal switch
        {
            Role => string.Format("GRANT {0} ON ACCOUNT TO ROLE {1};", flatPrivileges, principal.GetIdentifier()),
            DatabaseRole => throw new SnowflakeException("Account privileges cannot be granted to database roles"),
            _ => throw new NotImplementedException("GetGrantStatement is not implemented for this interface type")
        };
    }

    public string GetRevokeStatement(ISnowflakePrincipal principal, List<Privilege> privileges)
    {
        var flatPrivileges = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        return principal switch
        {
            Role => string.Format("REVOKE {0} ON ACCOUNT FROM ROLE {1} CASCADE;", flatPrivileges,
                principal.GetIdentifier()),
            DatabaseRole => throw new SnowflakeException(
                "Account privileges cannot be neither granted to nor revoked from database roles"),
            _ => throw new NotImplementedException("GetRevokeStatement is not implemented for this interface type")
        };
    }

    public bool ValidateGrants(List<Privilege> privileges)
    {
        throw new NotImplementedException();
    }
}