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
using Grant = Snowsharp.Client.Models.Entities.Grant;
using Role = Snowsharp.Client.Models.Describables.Role;
using Schema = Snowsharp.Client.Models.Assets.Schema;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class RoleGrantTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public RoleGrantTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    [Fact]
    public async Task Test_describe_grant_for_non_existing_role()
    {
        /*Arrange & Act*/
        var roleGrants = await _cli.ShowMany<Grant>(new Client.Models.Describables.Grant
        (
            new Role("I_DONT_EXIST_ROLE")
        ));

        /*Assert*/
        Assert.Empty(roleGrants);
    }

    [Fact]
    public async Task Test_role_account_grant()
    {
        /*Arrange*/
        var roleAsset = new Client.Models.Assets.Role("TEST_ROLE")
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var grant = new GrantAction(
            principal: roleAsset,
            target: new AccountGrant(),
            privileges: new List<Privilege> { Privilege.CreateDatabase, Privilege.CreateUser }
        );
        try
        {
            /*Act*/
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<Grant>(new Client.Models.Describables.Grant
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
    public async Task Test_role_database_grant()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var roleAsset = new Client.Models.Assets.Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var grant = new GrantAction(
            principal: roleAsset,
            target: new DatabaseGrant(dbAsset.Name),
            privileges: new List<Privilege> { Privilege.CreateDatabaseRole, Privilege.CreateSchema }
        );
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<Grant>(new Client.Models.Describables.Grant
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
    public async Task Test_role_schema_grant()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var schemaAsset = new Schema(dbAsset.Name, "TEST_SCHEMA")
        {
            Comment = "Integration test schema from the Snowsharp.Client test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var roleAsset = new Client.Models.Assets.Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var grant = new GrantAction(
            principal: roleAsset,
            target: new SchemaGrant(dbAsset.Name, schemaAsset.Name),
            privileges: new List<Privilege> { Privilege.Monitor, Privilege.Usage }
        );
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(schemaAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<Grant>(new Client.Models.Describables.Grant
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