using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;



[Collection("SnowflakeClientSetupCollection")]
public class SchemaTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public SchemaTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    [Fact]
    public async Task Test_create_schema()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var schemaAsset = new Schema(dbAsset.Name, "TEST_SCHEMA")
        {
            Comment = "Integration test schema from the Snowsharp.Client test suite",
            Owner = new Role("SYSADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(schemaAsset, _stack);
            var schemaEntity = await _cli.ShowOne<Client.Models.Entities.Schema>(
                new Client.Models.Describables.Schema(dbAsset.Name, schemaAsset.Name)
            );

            /*Assert*/
            Assert.NotNull(schemaEntity);
            Assert.Equal(schemaAsset.Name, schemaEntity!.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_describe_schema_in_non_existing_database()
    {
        /*Arrange & Act*/
        var schemaEntity = await _cli.ShowOne<Client.Models.Entities.Schema>(
            new Client.Models.Describables.Schema("DO_NOT_EXIST_DB", "DO_NOT_EXIST_SCHEMA")
        );


        /*Assert*/
        Assert.Null(schemaEntity);
    }



}