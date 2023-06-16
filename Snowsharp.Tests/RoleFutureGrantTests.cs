using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Assets.Grants;
using Snowsharp.Client.Models.Describables;
using Snowsharp.Client.Models.Entities;
using Snowsharp.Client.Models.Enums;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Database = Snowsharp.Client.Models.Assets.Database;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;
using FutureGrant = Snowsharp.Client.Models.Entities.FutureGrant;
using Role = Snowsharp.Client.Models.Describables.Role;
using Schema = Snowsharp.Client.Models.Assets.Schema;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class RoleFutureGrantTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public RoleFutureGrantTests(SnowSharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    [Fact]
    public async Task Test_describe_future_grant_for_non_existing_role()
    {
        /*Arrange & Act*/
        var roleGrants = await _cli.ShowMany<FutureGrant>(new Client.Models.Describables.FutureGrant
        (
            new Role("I_DONT_EXIST_ROLE")
        ));

        /*Assert*/
        Assert.Null(roleGrants);
    }

    [Fact]
    public async Task Test_role_future_database_object_grant()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var roleAsset = new Client.Models.Assets.Role("TEST_ROLE")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var grant = new GrantAction(
            principal: roleAsset,
            target: new DatabaseObjectFutureGrant(dbAsset.Name, SnowflakeObject.Table),
            privileges: new List<Privilege> { Privilege.Select, Privilege.References }
        );
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<FutureGrant>(new Client.Models.Describables.FutureGrant
            (
                new Role(roleAsset.Name)
            ));

            /*Assert*/
            Assert.NotNull(roleGrants);
            Assert.NotEmpty(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_role_schema_object_future_grant()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var schemaAsset = new Schema(dbAsset.Name, "TEST_SCHEMA")
        {
            Comment = "Integration test schema from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var roleAsset = new Client.Models.Assets.Role("TEST_ROLE")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var grant = new GrantAction(
            principal: roleAsset,
            target: new SchemaObjectFutureGrant(dbAsset.Name, schemaAsset.Name, SnowflakeObject.Table),
            privileges: new List<Privilege> { Privilege.Select, Privilege.References }
        );
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(schemaAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<FutureGrant>(new Client.Models.Describables.FutureGrant
            (
                new Role(roleAsset.Name)
            ));

            /*Assert*/
            Assert.NotNull(roleGrants);
            Assert.NotEmpty(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}