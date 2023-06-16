using System.Text.Json.Serialization;
using Snowsharp.Client.Converters;

namespace Snowsharp.Client.Models.Enums;


[JsonConverter(typeof(JsonStringEnumConverter<SnowflakeObject>))]
public enum SnowflakeObject
{
    [JsonPropertyName("TABLE")] Table,
    [JsonPropertyName("VIEW")] View,
    [JsonPropertyName("MATERIALIZED VIEW")] MatView,
    [JsonPropertyName("ACCOUNT")] Account,
    [JsonPropertyName("DATABASE")] Database,
    [JsonPropertyName("DATABASE_ROLE")] DatabaseRole,
    [JsonPropertyName("FUNCTION")] Function,
    [JsonPropertyName("ROLE")] Role,
    [JsonPropertyName("SCHEMA")] Schema,
    [JsonPropertyName("TAG")] Tag,
    [JsonPropertyName("USER")] User,
    [JsonPropertyName("SEQUENCE")] Sequence,
    [JsonPropertyName("PROCEDURE")] Procedure,
    [JsonPropertyName("FILE FORMAT")] FileFormat,
    [JsonPropertyName("INTERNAL STAGE")] InternalStage,
    [JsonPropertyName("EXTERNAL STAGE")] ExternalStage,
    [JsonPropertyName("PIPE")] Pipe,
    [JsonPropertyName("STREAM")] Stream,
    [JsonPropertyName("TASK")] Task,
    [JsonPropertyName("MASKING POLICY")] MaskingPolicy,
    [JsonPropertyName("PASSWORD POLICY")] PasswordPolicy,
    [JsonPropertyName("ROW ACCESS POLICY")] RowAccessPolicy,
    [JsonPropertyName("WAREHOUSE")] Warehouse,
    [JsonPropertyName("STAGE")] Stage, //InternalStage & ExternalStage?
}

public static class SnowflakeObjectValidator
{
    public static string ToSingularString(this SnowflakeObject value) => $"{value.ToString()}".ToUpper();

    public static string ToPluralString(this SnowflakeObject value)
    {
        return value switch
        {
            SnowflakeObject.MaskingPolicy => "MASKING POLICIES",
            SnowflakeObject.PasswordPolicy => "PASSWORD POLICIES",
            SnowflakeObject.RowAccessPolicy => "ROW ACCESS POLICIES",
            _ => $"{value.ToString()}S".ToUpper()
        };
    }

// public static bool Validate(this GrantTarget value, List<SnowflakePrivileges> privileges)
    // {
    //     return value switch
    //     {
    //         GrantTarget.Table => (new List<SnowflakePrivileges>()
    //         {
    //             SnowflakePrivileges.Select, 
    //             SnowflakePrivileges.Ownership
    //         }).Except(privileges).Any(),
    //         GrantTarget.View => (new List<SnowflakePrivileges>()
    //         {
    //             SnowflakePrivileges.Select, 
    //             SnowflakePrivileges.Ownership
    //         }).Except(privileges).Any(),
    //         _ => false
    //     };
    // }
}