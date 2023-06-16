namespace Snowsharp.Client.Models.Describables;

public class DatabaseRole:ISnowflakeDescribable, ISnowflakeGrantPrincipal
{
    public DatabaseRole(string databaseName, string name)
    {
        DatabaseName = databaseName;
        Name = name;
    }
    public string DatabaseName { get; init; }
    
    public string Name { get; init; }

    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW DATABASE ROLES LIKE '{0}' IN DATABASE {1};", Name, DatabaseName).ToUpper();
    }
}
