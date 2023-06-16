using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;


[Collection("SnowflakeClientSetupCollection")]
public class DatabaseRoleTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public DatabaseRoleTests(SnowSharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    [Fact]
    public async Task Test_create_database_role()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var roleAsset = new DatabaseRole("TEST_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            var dbRole = await _cli.ShowOne<Client.Models.Entities.DatabaseRole>(
            new Client.Models.Describables.DatabaseRole(dbAsset.Name, roleAsset.Name)
            );

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
    public async Task Test_describe_non_exising_database_role()
    {
        /*Arrange */
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);

            var dbRole = await _cli.ShowOne<Client.Models.Entities.DatabaseRole>(
                new Client.Models.Describables.DatabaseRole(dbAsset.Name, "I_SURELY_DO_NOT_EXIST_DATABASE_ROLE")
            );

            /*Assert*/
            Assert.Null(dbRole);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_describe_database_role_in_non_exising_database()
    {
        /*Arrange & Act*/
        var dbRole = await _cli.ShowOne<Client.Models.Entities.DatabaseRole>(
    new Client.Models.Describables.DatabaseRole("I_DONT_EXIST_DATABASE", "I_SURELY_DO_NOT_EXIST_DATABASE_ROLE")
           );

        /*Assert*/
        Assert.Null(dbRole);
    }

    [Fact]
    public async Task Test_database_role_with_database_role_owner()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var databaseRoleOwner = new DatabaseRole("TEST_OWNER_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var databaseRoleOwned = new DatabaseRole("TEST_OWNED_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = databaseRoleOwner
        };
        var rel = new RoleInheritance(childRole: databaseRoleOwner, parentPrincipal: new Role("USERADMIN"));
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(databaseRoleOwner, _stack);
            await _cli.RegisterAsset(rel, _stack);
            await _cli.RegisterAsset(databaseRoleOwned, _stack);
            var dbRole = await _cli.ShowOne<Client.Models.Entities.DatabaseRole>
            (
                new Client.Models.Describables.DatabaseRole(dbAsset.Name, databaseRoleOwned.Name)
            );

            /*Assert*/
            Assert.NotNull(dbRole);
            Assert.Equal(databaseRoleOwned.Name, dbRole!.Name);
            Assert.Equal(databaseRoleOwner.Name, databaseRoleOwner.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}