using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class RoleTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;
    private readonly string _comment;

    public RoleTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
        _comment = @"{""Comment"": ""Integration table test database from the SnowsharpClient test suite""}";
    }

    public async Task<(Database, Schema, Tag)> BootstrapRoleTagAssociationAssets()
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

    [Fact]
    public async Task Test_create_role()
    {
        /*Arrange*/
        var roleAsset = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = _comment,
            Owner = new Role("USERADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(roleAsset, _stack);
            var dbRole = await _cli.ShowOne<Entities.Role>(new Describables.Role(roleAsset.Name));

            /*Assert*/
            Assert.NotNull(dbRole);
            Assert.Equal(roleAsset.Name, dbRole!.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_describe_non_exising_role()
    {
        /*Arrange & Act*/
        var dbRole = await _cli.ShowOne<Entities.Role>(new Describables.Role("I_SURELY_DO_NOT_EXIST_ROLE"));

        /*Assert*/
        Assert.Null(dbRole);
    }

    [Fact]
    public async Task Test_create_role_with_tag_association()
    {
        var (_, _, tagAsset) = await BootstrapRoleTagAssociationAssets();
        var tagValue = tagAsset.TagValues.First();
        var roleAsset = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = _comment,
            Owner = new Role("USERADMIN"),
            Tags = new() { new(tagAsset.DatabaseName, tagAsset.SchemaName, tagAsset.TagName) { TagValue = tagValue } }
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(roleAsset, _stack);
            var dbRole = await _cli.ShowOne<Entities.Role>(new Describables.Role(roleAsset.Name));


            var tagAssociations = await _cli.ShowMany<Entities.TagAssociation>(
                new Describables.TagAssociation(roleAsset)
            );


            /*Assert*/
            //Role
            Assert.NotNull(dbRole);
            Assert.Equal(roleAsset.Name, dbRole!.Name);
            //Tag association
            Assert.NotEmpty(tagAssociations);
            Assert.Single(tagAssociations);
            Assert.Equal(tagAsset.DatabaseName, tagAssociations.First().TagDatabase);
            Assert.Equal(tagAsset.SchemaName, tagAssociations.First().TagSchema);
            Assert.Equal(tagAsset.TagName, tagAssociations.First().TagName);
            Assert.Equal(tagValue, tagAssociations.First().TagValue);
            Assert.Equal("ROLE", tagAssociations.First().Level);
            Assert.Null(tagAssociations.First().ObjectDatabase);
            Assert.Null(tagAssociations.First().ObjectSchema);
            Assert.Equal(roleAsset.Name, tagAssociations.First().ObjectName);
            Assert.Equal("ROLE", tagAssociations.First().Domain);
            Assert.Null(tagAssociations.First().ColumnName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}