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
using PrincipalAscendants = Snowsharp.Client.Models.Entities.PrincipalAscendants;
using Role = Snowsharp.Client.Models.Describables.Role;
using RoleInheritance = Snowsharp.Client.Models.Assets.RoleInheritance;


namespace Snowsharp.Tests;

[Collection("SnowflakeClientSetupCollection")]
public class PrincipalAscendantsTests
{
    private readonly SnowsharpClient _cli;
    private readonly Stack<ISnowflakeAsset> _stack;

    public PrincipalAscendantsTests(SnowsharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    /// <summary>
    /// We know that ACCOUNTADMIN is the direct ascendant of SYSADMIN 
    /// </summary>
    [Fact]
    public async Task Test_sysadmin_ascendants()
    {
        /*Arrange & Act*/
        var dbAscendants = await _cli.ShowOne<PrincipalAscendants>(new Client.Models.Describables.PrincipalAscendants(
            new Role("SYSADMIN")
        ));

        /*Assert*/
        Assert.NotNull(dbAscendants);
        Assert.NotNull(dbAscendants!.Ascendants);
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
        Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
    }

    // /// <summary>
    // /// We know that ACCOUNTADMIN is the direct ascendant of SECURITYADMIN
    // /// </summary>
    [Fact]
    public async Task Test_security_admin_ascendants()
    {
        /*Arrange & Act*/
        var dbAscendants = await _cli.ShowOne<PrincipalAscendants>(new Client.Models.Describables.PrincipalAscendants(
            new Role("SECURITYADMIN")
        ));

        /*Assert*/
        Assert.NotNull(dbAscendants);
        Assert.NotNull(dbAscendants!.Ascendants);
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
        Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
    }


    // /// <summary>
    // /// We know that ACCOUNTADMIN is the direct ascendant of SECURITYADMIN whom is the direct ascendant of USERADMIN
    // /// </summary>
    [Fact]
    public async Task Test_user_admin_ascendants()
    {
        /*Arrange & Act*/
        var dbAscendants = await _cli.ShowOne<PrincipalAscendants>(new Client.Models.Describables.PrincipalAscendants(
            new Role("USERADMIN")
        ));

        /*Assert*/
        Assert.NotNull(dbAscendants);
        Assert.NotNull(dbAscendants!.Ascendants);
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "SECURITYADMIN");
        Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "SECURITYADMIN").Select(y => y.DistanceFromSource).First());
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
        Assert.Equal(1, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
    }


    // /// <summary>
    // /// We know that ACCOUNTADMIN is the direct ascendant of SECURITYADMIN whom is the direct ascendant of USERADMIN
    // /// </summary>
    [Fact]
    public async Task Test_ascendants_with_database_roles()
    {
        /*Arrange*/
        var dbAsset = new Database($"TEST_SNOW_SHARP_CLIENT_DB_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test database from the SnowsharpClient test suite",
            Owner = new Client.Models.Assets.Role("SYSADMIN")
        };
        var dr1 = new DatabaseRole("TEST_SNOW_SHARP_CLIENT_DB_SYS_ADMIN", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var dr2 = new DatabaseRole("TEST_SNOW_SHARP_CLIENT_DB_SCHEMA_RWC", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var dr3 = new DatabaseRole("TEST_SNOW_SHARP_CLIENT_DB_SCHEMA_RW", dbAsset.Name)
        {
            Comment = "Integration test role from the Snowsharp.Client test suite",
            Owner = new Client.Models.Assets.Role("USERADMIN")
        };
        var rel1 = new RoleInheritance(childRole: dr1, parentPrincipal: new Client.Models.Assets.Role("SYSADMIN"));
        var rel2 = new RoleInheritance(childRole: dr2, parentPrincipal: dr1);
        var rel3 = new RoleInheritance(childRole: dr3, parentPrincipal: dr2);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(dr1, _stack);
            await _cli.RegisterAsset(dr2, _stack);
            await _cli.RegisterAsset(dr3, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            await _cli.RegisterAsset(rel3, _stack);

            var dbAscendants = await _cli.ShowOne<PrincipalAscendants>(new Client.Models.Describables.PrincipalAscendants(
                new Client.Models.Describables.DatabaseRole(dr3.DatabaseName, dr3.Name)
            ));

            /*Assert*/
            Assert.NotNull(dbAscendants);
            Assert.NotNull(dbAscendants!.Ascendants);
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == dr2.GetIdentifier());
            Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == dr2.GetIdentifier()).Select(y => y.DistanceFromSource).First());
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == dr1.GetIdentifier());
            Assert.Equal(1, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == dr1.GetIdentifier()).Select(y => y.DistanceFromSource).First());
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "SYSADMIN");
            Assert.Equal(2, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "SYSADMIN").Select(y => y.DistanceFromSource).First());
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
            Assert.Equal(3, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}


