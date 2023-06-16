using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

public class DatabaseRole : ISnowflakeAsset, ISnowflakePrincipal
{
    public DatabaseRole(string name, string databaseName)
    {
        Name = name;
        DatabaseName = databaseName;
    }

    public string Name { get; init; }
    public string DatabaseName { get; init; }
    public string Comment { get; init; } = "SNOW_SHARP_CLIENT TEST DATABASE ROLE";
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
CREATE OR REPLACE DATABASE ROLE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON DATABASE ROLE {0} TO {2} {3} REVOKE CURRENT GRANTS;",
            GetIdentifier(), Comment, ownerType.GetSnowflakeType(), Owner.GetIdentifier()
        );
    }

    public string GetDeleteStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("DROP DATABASE ROLE IF EXISTS {0};", GetIdentifier());
    }

    public string GetIdentifier()
    {
        return $"{DatabaseName}.{Name}";
    }
}