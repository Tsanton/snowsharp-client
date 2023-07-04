using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Tests.Fixtures;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public partial class TagAssociationTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;
    private readonly string _comment;

    public TagAssociationTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
        _comment = @"{""Comment"": ""Integration table test database from the SnowsharpClient test suite""}";
    }
    
    
    private async Task<(Database, Schema, Tag)> BootstrapTagAssociationAssets()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = _comment,
            Owner = new Role("SYSADMIN")
        };
        var schemaAsset = new Schema(dbAsset.Name, "TEST_SCHEMA")
        {
            Comment = _comment,
            Owner = new Role("SYSADMIN")
        };
        var tagAsset = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_TAG")
        {
            TagValues = new List<string> { "FOO", "BAR" },
            Owner = new Role("SYSADMIN"),
            Comment = _comment,
        }; 

        await _cli.RegisterAsset(dbAsset, _stack);
        await _cli.RegisterAsset(schemaAsset, _stack);
        await _cli.RegisterAsset(tagAsset, _stack);

        return (dbAsset, schemaAsset, tagAsset);
    }
}
