using System.Text;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Assets;

//https://hevodata.com/learn/snowflake-data-types/
public abstract class SnowflakeColumn
{
    protected SnowflakeColumn(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
    public bool PrimaryKey { get; init; }
    public bool Nullable { get; init; }
    public bool Unique { get; init; }
    public ForeignKey? ForeignKey  { get; init; }
    // MaskingPolicy MakingPolicy { get; init; }
    public List<ClassificationTag> Tags { get; init; } = new();

    public abstract string GetDefinition();
}

public class ForeignKey
{
    public ForeignKey(string databaseName, string schemaName, string tableName, string columnName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
        TableName = tableName;
        ColumnName = columnName;
    }
    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string TableName { get; init; }
    public string ColumnName { get; init; }
}

public class Identity
{
    public int StartNumber { get; init; } = 1;
    public int IncrementNumber { get; init; } = 1;

    public override string ToString()
    {
        return $"IDENTITY START {StartNumber} INCREMENT {IncrementNumber}";
    }
}

// public class MaskingPolicy
// {
//     public string PolicyName{ get; init; }
// }

public class ClassificationTag
{
    public ClassificationTag(string databaseName, string schemaName, string tagName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
        TagName = tagName;
    }

    public string DatabaseName{ get; init; }
    public string SchemaName{ get; init; }
    public string TagName{ get; init; }
    public string? TagValue{ get; init; }

    public string GetIdentifier()
    {
        return $"{DatabaseName}.{SchemaName}.{TagName}";
    }
}

// public class SequenceAssociation
// {
//     public string DatabaseName{ get; init; }
//     public string SchemaName{ get; init; }
//     public string SequenceName{ get; init; }
// }

public class Varchar: SnowflakeColumn
{
    public Varchar(string name) : base(name)
    {
        Name = name;
    }
    ///Constraint: 0 less or equal Length less or equal 16777216  
    public int Length { get; init; } = 16777216;
    public string? Collation { get; init; }
    public string? DefaultValue { get; init; }


    public override string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("VARCHAR").Append($"({Length})");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'");
        if (Collation is not null) sb.Append(' ').Append($"COLLATE '{Collation}'");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Number: SnowflakeColumn
{
    public Number(string name) : base(name)
    {
        Name = name;
    }
    ///Precision refers to `xxx` of xxx.yyy -> max 38
    public int Precision { get; init; } = 38;  
    //Scale refers to `yyy` of xxx.yyy -> max 37
    public int Scale { get; init; }
    public decimal? DefaultValue { get; init; }
    public Identity? Identity { get; init; }
    // public SequenceAssociation? Sequence { get; init; }
    public override string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("NUMBER").Append($"({Precision},{Scale})");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT {DefaultValue}");
        if (Identity is not null) sb.Append(' ').Append($"IDENTITY({Identity.StartNumber},{Identity.IncrementNumber})");
        // if (Sequence is not null) throw new NotImplementedException("Sequences are not supported as of now");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Bool: SnowflakeColumn
{
    public Bool(string name) : base(name)
    {
        Name = name;
    }
    public bool? DefaultValue { get; init; }

    public override string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("BOOLEAN");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT {DefaultValue.Value}");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}


public class Date: SnowflakeColumn
{
    public Date(string name) : base(name)
    {
        Name = name;
    }
    public string? DefaultValue { get; init; }
    public override string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append($"DATE");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Time: SnowflakeColumn
{
    public Time(string name) : base(name)
    {
        Name = name;
    }
    ///0 == seconds, 9 == nanoseconds 
    public int Scale { get; init; } //TODO: Validate 9 <= x <= 0
    public string? DefaultValue { get; init; }

    public override string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append($"TIME").Append($"({Scale})");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Timestamp: SnowflakeColumn
{
    public Timestamp(string name) : base(name)
    {
        Name = name;
    }
    public SnowflakeTimestamp TimestampType { get; init; } = SnowflakeTimestamp.LocalTimeZone;
    ///0 == seconds, 9 == nanoseconds 
    public int Scale { get; init; } //TODO: Validate 9 <= x <= 0
    public string? DefaultValue { get; init; }
    public override string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append(TimestampType.GetEnumJsonAttributeValue()).Append($"({Scale})");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Variant: SnowflakeColumn
{
    public Variant(string name) : base(name)
    {
        Name = name;
    }
    public string? DefaultValue { get; init; }

    public override string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("VARIANT");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}