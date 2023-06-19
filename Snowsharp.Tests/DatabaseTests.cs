using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;


namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class DatabaseTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public DatabaseTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    [Fact]
    public async Task Test_create_database()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            var dbEntity = await _cli.ShowOne<Client.Models.Entities.Database>(
                new Client.Models.Describables.Database(dbAsset.Name)
            );
            /*Assert*/
            Assert.NotNull(dbEntity);
            Assert.Equal(dbAsset.Name, dbEntity!.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_describe_non_existing_database()
    {
        /*Arrange & Act*/
        var dbEntity = await _cli.ShowOne<Client.Models.Entities.Database>(
            new Client.Models.Describables.Database("DO_NOT_EXIST_DB")
        );

        /*Assert*/
        Assert.Null(dbEntity);
    }
}