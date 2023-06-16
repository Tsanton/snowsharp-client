namespace Snowsharp.Client.Models.Describables;

public class Database: ISnowflakeDescribable
{
    public Database(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
    
    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW DATABASES LIKE '{0}';",Name).ToUpper();
    }
}