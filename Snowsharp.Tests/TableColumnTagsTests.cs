using Snowsharp.Client.Models.Assets;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;

public partial class TableTests
{
    [Fact]
    public async Task Test_create_table_with_column_tag_without_value()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var tagAsset = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_TAG")
        {
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
            Collation = null,
            Tags = new List<ClassificationTag>
            {
                new(tagAsset.DatabaseName, tagAsset.SchemaName, tagAsset.TagName)
                {
                    TagValue = null
                }
            },
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn> { col1, col2 },
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
            var taggedCol = dbTable.Columns.First(x => x.Name == col2.Name);
            Assert.Single(taggedCol.Tags);
            Assert.Equal(tagAsset.TagName, taggedCol.Tags.First().TagName);
            Assert.Null(taggedCol.Tags.First().TagValue);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_create_table_with_column_tag_with_value()
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
            Collation = null,
            Tags = new List<ClassificationTag>
            {
                new(tagAsset.DatabaseName, tagAsset.SchemaName, tagAsset.TagName)
                {
                    TagValue = "FOO"
                }
            },
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn> { col1, col2 },
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
            var taggedCol = dbTable.Columns.First(x => x.Name == col2.Name);
            Assert.Single(taggedCol.Tags);
            Assert.Equal(tagAsset.TagName, taggedCol.Tags.First().TagName);
            Assert.Equal("FOO", taggedCol.Tags.First().TagValue);

        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_create_table_with_multiple_column_tags()
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
            Collation = null,
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
            }
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn> { col1, col2 },
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
            var taggedCol = dbTable.Columns.First(x => x.Name == col2.Name);
            Assert.Equal(2, taggedCol.Tags.Count);
            Assert.Null(taggedCol.Tags.First(x => x.TagName == "TEST_TAG_1").TagValue);
            Assert.Equal("FOO", taggedCol.Tags.First(x => x.TagName == "TEST_TAG_2").TagValue);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_create_table_with_table_tag_and_multiple_column_tags()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var tagAsset1 = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_TABLE_TAG_1")
        {
            TagValues = new List<string>(),
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };
        var tagAsset2 = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_COLUMN_TAG_1")
        {
            TagValues = new List<string> { "FOO", "BAR" },
            Owner = new Role("SYSADMIN"),
            Comment = "SNOWPLOW TEST TAG"
        };
        var tagAsset3 = new Tag(dbAsset.Name, schemaAsset.Name, "TEST_COLUMN_TAG_2")
        {
            TagValues = new List<string> { "BAR", "BAZ" },
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
            Collation = null,
            Tags = new List<ClassificationTag>
            {
                new(tagAsset2.DatabaseName, tagAsset2.SchemaName, tagAsset2.TagName)
                {
                    TagValue = "FOO"
                },
                new(tagAsset3.DatabaseName, tagAsset3.SchemaName, tagAsset3.TagName)
                {
                    TagValue = "BAZ"
                }
            }
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn> { col1, col2 },
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE",
            Tags = new List<ClassificationTag>
            {
                new(tagAsset1.DatabaseName, tagAsset1.SchemaName, tagAsset1.TagName)
                {
                    TagValue = null
                }
            }
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tagAsset1, _stack);
            await _cli.RegisterAsset(tagAsset2, _stack);
            await _cli.RegisterAsset(tagAsset3, _stack);
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Client.Models.Entities.Table>(
                new Client.Models.Describables.Table(dbAsset.Name, schemaAsset.Name, tableAsset.TableName)
            );

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Equal(2, dbTable!.Columns.Count);
            Assert.Single(dbTable.Tags);
            Assert.Equal(tagAsset1.TagName, dbTable.Tags.First().TagName);
            Assert.Null(dbTable.Tags.First().TagValue);

            var taggedCol = dbTable.Columns.First(x => x.Name == col2.Name);
            Assert.Equal(3, taggedCol.Tags.Count); //Inherits one table tag, 
            Assert.Null(taggedCol.Tags.First(x => x.TagName == "TEST_TABLE_TAG_1").TagValue);
            Assert.Equal("FOO", taggedCol.Tags.First(x => x.TagName == "TEST_COLUMN_TAG_1").TagValue);
            Assert.Equal("BAZ", taggedCol.Tags.First(x => x.TagName == "TEST_COLUMN_TAG_2").TagValue);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}