using System.Text;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

public class Tag: ISnowflakeAsset
{
    public string? DatabaseName { get; init; }
    public string? SchemaName { get; init; }
    public string? TagName { get; init; }
    public List<string> TagValues { get; init; } = new();
    public ISnowflakePrincipal Owner { get; init; } = new Role("SYSADMIN");
    public string Comment { get; init; } = "SNOWPLOW TEST TAG";
    public string GetCreateStatement()
    {
        SnowflakePrincipal ownerType = Owner switch
        {
            Role => SnowflakePrincipal.Role,
            DatabaseRole => SnowflakePrincipal.DatabaseRole,
            _ => throw new NotImplementedException("Ownership is not implementer for this interface type")
        };
        var sb = new StringBuilder();
        sb.Append($"CREATE OR REPLACE TAG {DatabaseName}.{SchemaName}.{TagName}");
        if (TagValues is { Count: > 0 }) sb.Append(' ').Append("ALLOWED_VALUES").Append(' ').Append(string.Join(",", TagValues.Select(x => $"'{x}'")));
        sb.Append(' ').Append("COMMENT = ").Append($"'{Comment}'").AppendLine(";");
        sb.Append($"GRANT OWNERSHIP ON TAG {DatabaseName}.{SchemaName}.{TagName} TO {ownerType.GetSnowflakeType()} {Owner.GetIdentifier()}");
        return sb.ToString();
    }

    public string GetDeleteStatement()
    {
        return $"DROP TAG {DatabaseName}.{SchemaName}.{TagName}";
    }
}