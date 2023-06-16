namespace Snowsharp.Client.Models.Describables;

public class Role: ISnowflakeDescribable, ISnowflakeGrantPrincipal
{
    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; init; }

    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW ROLES LIKE '{0}';",Name).ToUpper();
    }
}