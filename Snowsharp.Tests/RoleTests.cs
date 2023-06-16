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

    public RoleTests(SnowSharpClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<ISnowflakeAsset>();
    }

    [Fact]
    public async Task Test_create_role()
    {
        /*Arrange*/
        var roleAsset = new Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = "Integration test role from the SnowSharp.Client test suite",
            Owner = new Role("USERADMIN")
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(roleAsset, _stack);
            var dbRole = await _cli.ShowOne<Client.Models.Entities.Role>(new Client.Models.Describables.Role(roleAsset.Name));

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
        var dbRole = await _cli.ShowOne<Client.Models.Entities.Role>(new Client.Models.Describables.Role("I_SURELY_DO_NOT_EXIST_ROLE"));

        /*Assert*/
        Assert.Null(dbRole);
    }
}