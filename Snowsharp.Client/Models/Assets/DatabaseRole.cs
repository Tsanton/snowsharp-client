using System.Text;
using Snowsharp.Client.Models.Commons;
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
        var sb = new StringBuilder();
        sb.Append($"CREATE OR REPLACE DATABASE ROLE {GetObjectIdentifier()}");
        sb.Append(' ').Append("COMMENT = ").Append($"'{Comment}'").AppendLine(";");
        sb.Append($"GRANT OWNERSHIP ON DATABASE ROLE {GetObjectIdentifier()} TO {ownerType.GetSnowflakeType()} {Owner.GetObjectIdentifier()}");
        return sb.ToString();
    }

    public string GetDeleteStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("DROP DATABASE ROLE IF EXISTS {0};", GetObjectIdentifier());
    }

    public string GetObjectIdentifier()
    {
        return $"{DatabaseName}.{Name}";
    }

    public string GetObjectType()
    {
        return "DATABASE_ROLE";
    }
}