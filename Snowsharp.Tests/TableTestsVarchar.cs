using Snowsharp.Client.Models.Assets;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;

public partial class TableTests
{
    [Fact]
    public async Task Test_table_varchar()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Varchar("VARCHAR_COLUMN")
        {
            PrimaryKey = false,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
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
            Assert.Equal("TEXT", dbTable.Columns.First().ColumnType.Type);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.Equal(col1.Length, dbTable.Columns.First().ColumnType.Length!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
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
    public async Task Test_table_varchar_primary_key()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Varchar("VARCHAR_COLUMN")
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
            Assert.Equal("TEXT", dbTable.Columns.First().ColumnType.Type);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.Equal(col1.Length, dbTable.Columns.First().ColumnType.Length!.Value);
            Assert.True(dbTable.Columns.First().PrimaryKey);
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
    public async Task Test_table_varchar_multiple_columns()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Varchar("VARCHAR_COLUMN_1")
        {
            PrimaryKey = false,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var col2 = new Varchar("VARCHAR_COLUMN_2")
        {
            PrimaryKey = false,
            Length = 16777216,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Collation = null
        };
        var tableAsset = new Table(dbAsset.Name, schemaAsset.Name, "TEST_TABLE")
        {
            Columns = new List<SnowflakeColumn>{col1, col2},
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
            Assert.Equal(2, dbTable!.Columns.Count);
            var dbCol = dbTable.Columns.First(x => x.Name == col1.Name);
            Assert.Equal(col1.Name, dbCol.Name);
            Assert.Equal("TEXT", dbCol.ColumnType.Type);
            Assert.False(dbCol.ColumnType.Nullable);
            Assert.Equal(col1.Length, dbCol.ColumnType.Length!.Value);
            Assert.False(dbCol.PrimaryKey);
            Assert.False(dbCol.UniqueKey);
            Assert.Null(dbCol.Default);
            Assert.Null(dbCol.Expression);
            Assert.Null(dbCol.Check);
            Assert.Null(dbCol.PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }

    [Fact]
    public async Task Test_table_varchar_composite_primary_key()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
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
            Columns = new List<SnowflakeColumn>{col1, col2},
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
            Assert.Equal(2, dbTable!.Columns.Count);
            var dbCol = dbTable.Columns.First(x => x.Name == col1.Name);
            Assert.Equal(col1.Name, dbCol.Name);
            Assert.Equal("TEXT", dbCol.ColumnType.Type);
            Assert.False(dbCol.ColumnType.Nullable);
            Assert.Equal(col1.Length, dbCol.ColumnType.Length!.Value);
            Assert.True(dbCol.PrimaryKey);
            Assert.False(dbCol.UniqueKey);
            Assert.Null(dbCol.Default);
            Assert.Null(dbCol.Expression);
            Assert.Null(dbCol.Check);
            Assert.Null(dbCol.PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}