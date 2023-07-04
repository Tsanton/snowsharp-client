using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;


namespace Snowsharp.Tests;

public partial class TagAssociationTests
{
    [Fact]
    public async Task Test_associate_single_tag_to_role()
    {
        /*Arrange*/
        var (_, _, tagAsset) = await BootstrapTagAssociationAssets();
        var roleAsset = new Assets.Role($"TEST_ROLE_{Guid.NewGuid():N}".ToUpper())
        {
            Comment = _comment,
            Owner = new Assets.Role("USERADMIN")
        };
        var tagValue = tagAsset.TagValues.First();
        var tagRoleAssociation = new Assets.TagAssociation(roleAsset, tagAsset, tagValue);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(tagRoleAssociation, _stack);

            var tagAssociations = await _cli.ShowMany<Entities.TagAssociation>(
                new Describables.TagAssociation(roleAsset)
            );


            /*Assert*/
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