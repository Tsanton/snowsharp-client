using System.Data;
using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Describables;
using Snowsharp.Client.Models.Entities;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Database = Snowsharp.Client.Models.Assets.Database;
using DatabaseRole = Snowsharp.Client.Models.Describables.DatabaseRole;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;
using Role = Snowsharp.Client.Models.Assets.Role;
using RoleInheritance = Snowsharp.Client.Models.Entities.RoleInheritance;

namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class RoleInheritanceTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public RoleInheritanceTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    #region RoleToRole
    [Fact]
    public async Task Test_show_non_existing_role_to_role_inheritance()
    {
        /*Arrange*/
        var r1 = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var r2 = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(r1, _stack);
            await _cli.RegisterAsset(r2, _stack);
            var dbInheritance = await _cli.ShowOne<RoleInheritance>(new Client.Models.Describables.RoleInheritance
            (
                inheritedRole: new Client.Models.Describables.Role(r1.Name),
                parentPrincipal: new Client.Models.Describables.Role(r2.Name)
            ));

            /*Assert*/
            Assert.Null(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_show_role_to_role_inheritance()
    {
        /*Arrange*/
        var r1 = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var r2 = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var rel = new Client.Models.Assets.RoleInheritance(childRole: r1, parentPrincipal: r2);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(r1, _stack);
            await _cli.RegisterAsset(r2, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbInheritance = await _cli.ShowOne<RoleInheritance>(new Client.Models.Describables.RoleInheritance
            (
                inheritedRole: new Client.Models.Describables.Role(r1.Name),
                parentPrincipal: new Client.Models.Describables.Role(r2.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion

    #region RoleToDatabaseRole
    /// <summary>
    /// Account roles can not be granted to database roles
    /// </summary>
    [Fact]
    public async Task Test_show_role_to_database_role_inheritance_in_non_existing_database()
    {
        /*Arrange*/
        var r1 = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(r1, _stack);
            var dbInheritance = await _cli.ShowOne<RoleInheritance>(new Client.Models.Describables.RoleInheritance
            (
                inheritedRole: new DatabaseRole("I_DONT_EXIST_DATABASE", "I_DONT_EXIST_ROLE"),
                parentPrincipal: new Client.Models.Describables.Role(r1.Name)
            ));

            /*Assert*/
            Assert.Null(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    /// <summary>
    /// Account roles can not be granted to database roles
    /// </summary>
    [Fact]
    public async Task Test_show_role_to_database_role_inheritance()
    {
        /* Arrange */
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var r1 = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var dr1 = new Client.Models.Assets.DatabaseRole("TEST_SNOWPLOW_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var rel = new Client.Models.Assets.RoleInheritance(childRole: r1, parentPrincipal: dr1);

        /* Act and Assert */
        await Assert.ThrowsAsync<ConstraintException>(() => _cli.RegisterAsset(rel, _stack));
    }
    #endregion


    #region DatabaseRoleToRole
    [Fact]
    public async Task Test_show_database_role_to_role_inheritance()
    {
        /* Arrange */
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var dr1 = new Client.Models.Assets.DatabaseRole("TEST_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var r1 = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var rel = new Client.Models.Assets.RoleInheritance(childRole: dr1, parentPrincipal: r1);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(dr1, _stack);
            await _cli.RegisterAsset(r1, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbInheritance = await _cli.ShowOne<RoleInheritance>(new Client.Models.Describables.RoleInheritance
            (
                inheritedRole: new DatabaseRole(dr1.DatabaseName, dr1.Name),
                parentPrincipal: new Client.Models.Describables.Role(r1.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion

    #region DatabaseRoleToDatabaseRole
    [Fact]
    public async Task Test_show_database_role_to_database_role_cross_database_inheritance()
    {
        /* Arrange */
        var db1 = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var db2 = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var dr1 = new Client.Models.Assets.DatabaseRole("TEST_DATABASE_ROLE1", db1.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var dr2 = new Client.Models.Assets.DatabaseRole("TEST_DATABASE_ROLE2", db2.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var rel = new Client.Models.Assets.RoleInheritance(childRole: dr1, parentPrincipal: dr2);

        /* Act and Assert */
        await Assert.ThrowsAsync<ConstraintException>(() => _cli.RegisterAsset(rel, _stack));
    }

    [Fact]
    public async Task Test_show_database_role_to_database_role_same_database_inheritance()
    {
        /* Arrange */
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Role("SYSADMIN")
        };
        var dr1 = new Client.Models.Assets.DatabaseRole("TEST_DATABASE_ROLE1", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var dr2 = new Client.Models.Assets.DatabaseRole("TEST_DATABASE_ROLE2", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        var rel = new Client.Models.Assets.RoleInheritance(childRole: dr1, parentPrincipal: dr2);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(dr1, _stack);
            await _cli.RegisterAsset(dr2, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbInheritance = await _cli.ShowOne<RoleInheritance>(new Client.Models.Describables.RoleInheritance
            (
                inheritedRole: new DatabaseRole(dr1.DatabaseName, dr1.Name),
                parentPrincipal: new DatabaseRole(dr2.DatabaseName, dr2.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion
}

