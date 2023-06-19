using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Assets.Grants;
using Snowsharp.Client.Models.Enums;
using Snowsharp.Tests.Fixtures;
using Database = Snowsharp.Client.Models.Assets.Database;
using DatabaseRole = Snowsharp.Client.Models.Describables.DatabaseRole;
using FutureGrant = Snowsharp.Client.Models.Entities.FutureGrant;
using Role = Snowsharp.Client.Models.Assets.Role;
using Schema = Snowsharp.Client.Models.Assets.Schema;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class DatabaseRoleFutureGrantTests
{
    private readonly Client.SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public DatabaseRoleFutureGrantTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    [Fact]
    public async Task Test_describe_future_grant_for_non_existing_database_role()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("USERADMIN")
        };
        /*Act*/
        try
        {
            await _cli.RegisterAsset(dbAsset, _stack);

            /*Act*/
            var roleGrants = await _cli.ShowMany<FutureGrant>(new Client.Models.Describables.FutureGrant(
            new DatabaseRole(dbAsset.Name, "I_DONT_EXIST_DATABASE_ROLE"))
            );

            /*Assert*/
            Assert.Null(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_describe_future_grant_for_database_role_in_non_existing_database()
    {
        /*Arrange & Act*/
        try
        {
            var roleGrants = await _cli.ShowMany<FutureGrant>(new Client.Models.Describables.FutureGrant(
                new DatabaseRole("I_DONT_EXIST_DATABASE", "I_DONT_EXIST_EITHER_DATABASE_ROLE"))
            );

            /*Assert*/
            Assert.Null(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_database_role_future_database_object_grant()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var roleAsset = new Client.Models.Assets.DatabaseRole("TEST_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Role("USERADMIN")
        };
        var grant = new DatabaseObjectFutureGrant(dbAsset.Name, SnowflakeObject.Table);
        var grantAction = new GrantAction(
            principal: roleAsset,
            target: grant,
            privileges: new List<Privilege> { Privilege.Select, Privilege.References }
        );
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grantAction, _stack);
            var roleGrants = await _cli.ShowMany<FutureGrant>(new Client.Models.Describables.FutureGrant(
                new DatabaseRole(dbAsset.Name, roleAsset.Name))
            );

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
    public async Task Test_database_role_schema_object_future_grant()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var schemaAsset = new Schema(dbAsset.Name, "TEST_SCHEMA")
        {
            Comment = "Integration test schema from the Snowplow test suite",
            Owner = new Role("SYSADMIN")
        };
        var roleAsset = new Client.Models.Assets.DatabaseRole("TEST_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Role("USERADMIN")
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
            var roleGrants = await _cli.ShowMany<FutureGrant>(new Client.Models.Describables.FutureGrant(
                new DatabaseRole(dbAsset.Name, roleAsset.Name))
            );

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