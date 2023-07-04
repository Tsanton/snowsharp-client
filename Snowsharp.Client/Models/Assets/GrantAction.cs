using Snowsharp.Client.Models.Assets.Grants;
using Snowsharp.Client.Models.Commons;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

public class GrantAction : ISnowflakeAsset
{
    public GrantAction(ISnowflakePrincipal principal, ISnowflakeGrantAsset target, List<Privilege> privileges)
    {
        Principal = principal;
        Target = target;
        Privileges = privileges;
    }

    public ISnowflakePrincipal Principal { get; init; }
    public ISnowflakeGrantAsset Target { get; init; }
    public List<Privilege> Privileges { get; set; }

    public string GetCreateStatement()
    {
        return Target.GetGrantStatement(Principal, Privileges);
    }

    public string GetDeleteStatement()
    {
        return Target.GetRevokeStatement(Principal, Privileges);
    }
}
