using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

public class Role : ISnowflakeAsset, ISnowflakePrincipal
{
    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
    public string Comment { get; init; } = "SNOWPLOW TEST ROLE";
    public ISnowflakePrincipal? Owner { get; init; }
    public string GetCreateStatement()
    {
        var ownerType = Owner switch
        {
            Role => SnowflakePrincipal.Role,
            _ => throw new NotImplementedException("Ownership is not implementer for this interface type"),
        };
        return string.Format(@"
CREATE OR REPLACE ROLE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON ROLE {0} TO {2} {3} REVOKE CURRENT GRANTS;",
            GetIdentifier(), Comment, ownerType.GetSnowflakeType(), Owner.GetIdentifier()
        );
    }

    public string GetDeleteStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("DROP ROLE IF EXISTS {0};", GetIdentifier());
    }

    public string GetIdentifier()
    {
        return Name;
    }
}