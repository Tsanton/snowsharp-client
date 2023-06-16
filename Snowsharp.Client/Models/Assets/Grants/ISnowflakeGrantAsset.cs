using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets.Grants;

public interface ISnowflakeGrantAsset
{
    public string GetGrantStatement(ISnowflakePrincipal principal, List<Privilege> privileges);
    public string GetRevokeStatement(ISnowflakePrincipal principal, List<Privilege> privileges);
    protected bool ValidateGrants(List<Privilege> privileges);
}