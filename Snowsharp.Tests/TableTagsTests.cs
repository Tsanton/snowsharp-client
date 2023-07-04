using Snowsharp.Client.Models.Assets;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;
namespace Snowsharp.Tests;

public partial class TableTests
{
    [Fact]
    public async Task Test_create_table_with_tag_without_value()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var tagAsset = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_TAG")
        {
            // TagValues = new List<string>{"FOO", "BAR"},
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };
        var col1 = new Varchar("VARCHAR_COLUMN_1")
        {
            PrimaryKey = true,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var col2 = new Varchar("VARCHAR_COLUMN_2")
        {
            PrimaryKey = true,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn> { col1, col2 },
            Tags = new List<ClassificationTag>
            {
                new(tagAsset.DatabaseName, tagAsset.SchemaName, tagAsset.TagName)
                {
                    TagValue = null
                }
            },
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tagAsset, _stack);
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Client.Models.Entities.Table>(
                new Client.Models.Describables.Table(dbAsset.Name, schemaAsset.Name, tableAsset.TableName)
            );

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Equal(2, dbTable!.Columns.Count);
            Assert.Single(dbTable.Tags);
            Assert.Equal(tagAsset.TagName, dbTable.Tags.First().TagName);
            Assert.Null(dbTable.Tags.First().TagValue);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_create_table_with_tag_with_value()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var tagAsset = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_TAG")
        {
            TagValues = new List<string> { "FOO", "BAR" },
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };
        var col1 = new Varchar("VARCHAR_COLUMN_1")
        {
            PrimaryKey = true,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var col2 = new Varchar("VARCHAR_COLUMN_2")
        {
            PrimaryKey = true,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn> { col1, col2 },
            Tags = new List<ClassificationTag>
            {
                new(tagAsset.DatabaseName, tagAsset.SchemaName, tagAsset.TagName)
                {
                    DatabaseName = tagAsset.DatabaseName,
                    SchemaName = tagAsset.SchemaName,
                    TagName = tagAsset.TagName,
                    TagValue = "FOO"
                }
            },
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tagAsset, _stack);
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Client.Models.Entities.Table>(
                new Client.Models.Describables.Table(dbAsset.Name, schemaAsset.Name, tableAsset.TableName)
            );

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Equal(2, dbTable!.Columns.Count);
            Assert.Single(dbTable.Tags);
            Assert.Equal(tagAsset.TagName, dbTable.Tags.First().TagName);
            Assert.Equal("FOO", dbTable.Tags.First().TagValue);

        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_create_table_with_multiple_tags()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var tagAsset1 = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_TAG_1")
        {
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };
        var tagAsset2 = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_TAG_2")
        {
            TagValues = new List<string> { "FOO", "BAR" },
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };
        var col1 = new Varchar("VARCHAR_COLUMN_1")
        {
            PrimaryKey = true,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var col2 = new Varchar("VARCHAR_COLUMN_2")
        {
            PrimaryKey = true,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn> { col1, col2 },
            Tags = new List<ClassificationTag>
            {
                new(tagAsset1.DatabaseName, tagAsset1.SchemaName, tagAsset1.TagName)
                {
                    TagValue = null
                },
                new(tagAsset2.DatabaseName, tagAsset2.SchemaName, tagAsset2.TagName)
                {
                    TagValue = "FOO"
                }
            },
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tagAsset1, _stack);
            await _cli.RegisterAsset(tagAsset2, _stack);
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Client.Models.Entities.Table>(
                new Client.Models.Describables.Table(dbAsset.Name, schemaAsset.Name, tableAsset.TableName)
            );

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Equal(2, dbTable!.Columns.Count);
            Assert.Equal(2, dbTable.Tags.Count);
            Assert.Equal(tagAsset1.TagName, dbTable.Tags.First(x => x.TagName == "TEST_TAG_1").TagName);
            Assert.Null(dbTable.Tags.First(x => x.TagName == "TEST_TAG_1").TagValue);
            Assert.Equal(tagAsset2.TagName, dbTable.Tags.First(x => x.TagName == "TEST_TAG_2").TagName);
            Assert.Equal("FOO", dbTable.Tags.First(x => x.TagName == "TEST_TAG_2").TagValue);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}