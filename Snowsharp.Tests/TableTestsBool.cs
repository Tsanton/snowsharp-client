using Snowsharp.Client.Models.Assets;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;

public partial class TableTests
{
    [Fact]
    public async Task Test_table_bool()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Bool("BOOL_COLUMN")
        {
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn>{col1},
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Client.Models.Entities.Table>(
                new Client.Models.Describables.Table(dbAsset.Name, schemaAsset.Name, tableAsset.TableName)
            );

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Single(dbTable!.Columns);
            Assert.Equal("BOOLEAN", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Name, dbTable.Columns.First().Name);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Null(dbTable.Columns.First().Default);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_table_bool_primary_key()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Bool("BOOL_COLUMN")
        {
            PrimaryKey = true,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn>{col1},
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Client.Models.Entities.Table>(
                new Client.Models.Describables.Table(dbAsset.Name, schemaAsset.Name, tableAsset.TableName)
            );

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Single(dbTable!.Columns);
            Assert.Equal(col1.Name, dbTable.Columns.First().Name);
            Assert.Equal("BOOLEAN", dbTable.Columns.First().ColumnType.Type);
            Assert.True(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Null(dbTable.Columns.First().Default);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}
