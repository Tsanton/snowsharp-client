using Snowsharp.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Describables;
using Snowsharp.Client.Models.Entities;
using Snowsharp.Tests.Fixtures;
using Assets = Snowsharp.Client.Models.Assets;
using Database = Snowsharp.Client.Models.Assets.Database;
using DatabaseRole = Snowsharp.Client.Models.Assets.DatabaseRole;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;
using PrincipalDescendants = Snowsharp.Client.Models.Entities.PrincipalDescendants;
using Role = Snowsharp.Client.Models.Describables.Role;
using RoleInheritance = Snowsharp.Client.Models.Assets.RoleInheritance;


namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class PrincipalDescendantsTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public PrincipalDescendantsTests(SnowSharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    /// <summary>
    /// We know that SYSADMIN and SECURITYADMIN are direct descendants of ACCOUNTADMIN
    /// </summary>
    [Fact]
    public async Task Test_account_admin_descendants()
    {
        /*Arrange & Act*/
        var dbDescendants = await _cli.ShowOne<PrincipalDescendants>(new Client.Models.Describables.PrincipalDescendants
        (
             new Role("ACCOUNTADMIN")
        ));

        /*Assert*/
        Assert.NotNull(dbDescendants);
        Assert.NotNull(dbDescendants!.Descendants);
        Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == "SYSADMIN");
        Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == "SECURITYADMIN");
    }

    [Fact]
    public async Task Test_role_to_role_descendants()
    {
        /*Arrange*/
        var childRole = new Client.Models.Assets.Role("TEST_CHILD_ROLE")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var parentRole = new Client.Models.Assets.Role("TEST_PARENT_ROLE")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var rel = new RoleInheritance(childRole: childRole, parentPrincipal: parentRole);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(childRole, _stack);
            await _cli.RegisterAsset(parentRole, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbDescendants = await _cli.ShowOne<PrincipalDescendants>(new Client.Models.Describables.PrincipalDescendants
            (
                new Role(parentRole.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Single(dbDescendants!.Descendants);
            Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == childRole.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_role_to_roles_descendants()
    {
        /*Arrange*/
        var childRole1 = new Client.Models.Assets.Role("TEST_CHILD_ROLE_1")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var childRole2 = new Client.Models.Assets.Role("TEST_CHILD_ROLE_2")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var parentRole = new Client.Models.Assets.Role("TEST_PARENT_ROLE")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };

        var rel1 = new RoleInheritance(childRole: childRole1, parentPrincipal: parentRole);
        var rel2 = new RoleInheritance(childRole: childRole2, parentPrincipal: parentRole);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(childRole1, _stack);
            await _cli.RegisterAsset(childRole2, _stack);
            await _cli.RegisterAsset(parentRole, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            var dbDescendants = await _cli.ShowOne<PrincipalDescendants>(new Client.Models.Describables.PrincipalDescendants
            (
                new Role(parentRole.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Equal(2, dbDescendants!.Descendants.Count);
            Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == childRole1.Name);
            Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == childRole2.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }


    [Fact]
    public async Task Test_role_to_role_and_database_role_descendants()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var childRole = new Client.Models.Assets.Role("TEST_CHILD_ROLE")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var childDatabaseRole = new DatabaseRole("TEST_SNOWPLOW_CHILD_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var parentRole = new Client.Models.Assets.Role("TEST_PARENT_ROLE")
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var rel1 = new RoleInheritance(childRole: childRole, parentPrincipal: parentRole);
        var rel2 = new RoleInheritance(childRole: childDatabaseRole, parentPrincipal: parentRole);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(childRole, _stack);
            await _cli.RegisterAsset(childDatabaseRole, _stack);
            await _cli.RegisterAsset(parentRole, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            var dbDescendants = await _cli.ShowOne<PrincipalDescendants>(new Client.Models.Describables.PrincipalDescendants
            (
                new Role(parentRole.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Equal(2, dbDescendants!.Descendants.Count);
            Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == childRole.Name);
            Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == childDatabaseRole.GetIdentifier());
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_database_role_to_database_roles_descendants()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowSharpClient test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var childDatabaseRole1 = new DatabaseRole("TEST_SNOWPLOW_CHILD_DATABASE_ROLE1", dbAsset.Name)
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var childDatabaseRole2 = new DatabaseRole("TEST_SNOWPLOW_CHILD_DATABASE_ROLE2", dbAsset.Name)
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var parentDatabaseRole = new DatabaseRole("TEST_SNOWPLOW_PARENT_DATABASE_ROLE", dbAsset.Name)
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var rel1 = new RoleInheritance(childRole: childDatabaseRole1, parentPrincipal: parentDatabaseRole);
        var rel2 = new RoleInheritance(childRole: childDatabaseRole2, parentPrincipal: parentDatabaseRole);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(parentDatabaseRole, _stack);
            await _cli.RegisterAsset(childDatabaseRole1, _stack);
            await _cli.RegisterAsset(childDatabaseRole2, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            var dbDescendants = await _cli.ShowOne<PrincipalDescendants>(new Client.Models.Describables.PrincipalDescendants
            (
                new Client.Models.Describables.DatabaseRole(parentDatabaseRole.DatabaseName, parentDatabaseRole.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Equal(2, dbDescendants!.Descendants.Count);
            Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == childDatabaseRole1.GetIdentifier());
            Assert.Contains(dbDescendants.Descendants, x => x.GrantedIdentifier == childDatabaseRole2.GetIdentifier());
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}