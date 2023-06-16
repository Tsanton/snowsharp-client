using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Enums;
using Assets = Snowsharp.Client.Models.Assets;
using Entities = Snowsharp.Client.Models.Entities;
using Describables = Snowsharp.Client.Models.Describables;

namespace Snowsharp.Tests;

public partial class TableTests
{
    [Fact]
    public async Task test_table_timestamp()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Timestamp("TIMESTAMP_COLUMN")
        {
            TimestampType = SnowflakeTimestamp.LocalTimeZone,
            Scale = 0,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            DefaultValue = null,
            ForeignKey = null
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
            Assert.Equal(SnowflakeTimestamp.LocalTimeZone.GetEnumJsonAttributeValue(), dbTable.Columns.First().ColumnType.Type);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.Equal(col1.Scale, dbTable.Columns.First().ColumnType.Precision!.Value);
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
}