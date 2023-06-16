using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Entities;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Database = Snowsharp.Client.Models.Assets.Database;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;
using Role = Snowsharp.Client.Models.Assets.Role;
using Schema = Snowsharp.Client.Models.Assets.Schema;
using Tag = Snowsharp.Client.Models.Entities.Tag;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class TagTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public TagTests(SnowSharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    private async Task<(Database, Schema)> BootstrapTagAssets()
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

    [Fact]
    public async Task Test_describe_non_existing_tag()
    {
        /*Arrange & Act*/
        var tag = await _cli.ShowOne<Tag>(
    new Client.Models.Describables.Tag("SNOWFLAKE", "ACCOUNT_USAGE", "I_DONT_EXIST_TAG")
        );

        /*Assert*/
        Assert.Null(tag);
    }

    [Fact]
    public async Task Test_create_tag_without_tag_values()
    {
        /*Arrange */
        var (dbAsset, schemaAsset) = await BootstrapTagAssets();
        var tagAsset = new Client.Models.Assets.Tag
        {
            DatabaseName = dbAsset.Name,
            SchemaName = schemaAsset.Name,
            TagName = "TEST_TAG",
            // TagValues = null,
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };

        /* Act */
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tagAsset, _stack);

            var dbTag = await _cli.ShowOne<Tag>(
            new Client.Models.Describables.Tag(dbAsset.Name, schemaAsset.Name, tagAsset.TagName)
            );

            /*Assert*/
            Assert.NotNull(dbTag);
            Assert.Equal(tagAsset.TagName, dbTag!.Name);
            Assert.Empty(tagAsset.TagValues);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_create_tag_with_tag_values()
    {
        /*Arrange */
        var (dbAsset, schemaAsset) = await BootstrapTagAssets();
        var tagAsset = new Client.Models.Assets.Tag
        {
            DatabaseName = dbAsset.Name,
            SchemaName = schemaAsset.Name,
            TagName = "TEST_TAG",
            TagValues = new List<string> { "FOO", "BAR" },
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };

        /* Act */
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tagAsset, _stack);
            var dbTag = await _cli.ShowOne<Tag>(
                new Client.Models.Describables.Tag(dbAsset.Name, schemaAsset.Name, tagAsset.TagName)
            );

            /*Assert*/
            Assert.NotNull(dbTag);
            Assert.Equal(tagAsset.TagName, dbTag!.Name);
            Assert.Equal(2, tagAsset.TagValues.Count);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}