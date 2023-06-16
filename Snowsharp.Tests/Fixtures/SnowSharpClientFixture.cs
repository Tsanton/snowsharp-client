using Snowflake.Client;
using Snowflake.Client.Model;
using Snowsharp.Client;

namespace Snowsharp.Tests.Fixtures;

[CollectionDefinition("SnowflakeClientSetupCollection")]
public class DatabaseCollection : ICollectionFixture<SnowSharpClientFixture>
{
    // A class with no code, only used to define the collection
}

public class SnowSharpClientFixture: IDisposable
{
    public readonly SnowsharpClient Plow;

    public SnowSharpClientFixture()
    {
        Console.WriteLine("Setting up");
        var cli = new SnowflakeClient(
            authInfo: new AuthInfo
            {
                Account = Environment.GetEnvironmentVariable("SNOWFLAKE_ACCOUNT"),
                Password = Environment.GetEnvironmentVariable("SNOWFLAKE_PWD"),
                User = Environment.GetEnvironmentVariable("SNOWFLAKE_UID"),
                Region = Environment.GetEnvironmentVariable("SNOWFLAKE_REGION"),
            },
            urlInfo: new UrlInfo
            {
                Host = Environment.GetEnvironmentVariable("SNOWFLAKE_HOST"),
                Protocol = "https",
                Port = 443
            },
            sessionInfo: new SessionInfo
            {
                Role = Environment.GetEnvironmentVariable("SNOWFLAKE_ROLE"),
                Warehouse = Environment.GetEnvironmentVariable("SNOWFLAKE_WH")
            });

        Plow = new SnowsharpClient(cli);
    }
    
    public void Dispose()
    {
        Console.WriteLine("Disposing");
    }
}