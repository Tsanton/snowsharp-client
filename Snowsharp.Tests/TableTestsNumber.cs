using Snowsharp.Client.Models.Assets;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;

public partial class TableTests
{
    #region integer
    [Fact]
    public async Task Test_table_integer()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Number("INTEGER_COLUMN")
        {
            Precision = 38,
            Scale = 0,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Identity = null
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
            Assert.Equal("FIXED", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Precision, dbTable.Columns.First().ColumnType.Precision!.Value);
            Assert.Equal(col1.Scale, dbTable.Columns.First().ColumnType.Scale!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Null(dbTable.Columns.First().Default);
            Assert.Null(dbTable.Columns.First().AutoIncrement);
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
    public async Task Test_table_integer_auto_increment()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Number("INTEGER_COLUMN")
        {
            Precision = 38,
            Scale = 0,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Identity = new Identity
            {
                StartNumber = 0,
                IncrementNumber = 1
            }
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
            Assert.Equal("FIXED", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Precision, dbTable.Columns.First().ColumnType.Precision!.Value);
            Assert.Equal(col1.Scale, dbTable.Columns.First().ColumnType.Scale!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Equal(col1.Identity.ToString(), dbTable.Columns.First().Default);
            Assert.Equal(col1.Identity.ToString(), dbTable.Columns.First().AutoIncrement);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion

    #region decimal
    [Fact]
    public async Task Test_table_decimal()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Number("NUMBER_COLUMN")
        {
            Precision = 38,
            Scale = 37,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Identity = null
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
            Assert.Equal("FIXED", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Precision, dbTable.Columns.First().ColumnType.Precision!.Value);
            Assert.Equal(col1.Scale, dbTable.Columns.First().ColumnType.Scale!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Null(dbTable.Columns.First().Default);
            Assert.Null(dbTable.Columns.First().AutoIncrement);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion
}