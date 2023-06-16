using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public partial class TableTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public TableTests(SnowSharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    private async Task<(Database, Schema)> BootstrapTableAssets()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var schemaAsset = new Schema(dbAsset.Name, "TEST_SCHEMA")
        {
            Comment = "Integration test schema from the SnowSharp.Client test suite",
            Owner = new Role("SYSADMIN")
        };

        await _cli.RegisterAsset(dbAsset, _stack);
        await _cli.RegisterAsset(schemaAsset, _stack);

        return (dbAsset, schemaAsset);
    }
}