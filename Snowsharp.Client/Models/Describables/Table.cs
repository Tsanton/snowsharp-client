using Snowsharp.Client.Models.Commons;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;

public class Table : ISnowflakeDescribable, ISnowflakeTaggable
{
    public Table(string databaseName, string schemaName, string tableName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
        TableName = tableName;
    }

    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string TableName { get; init; }
    public string GetDescribeStatement()
    {
        var query = $@"
with show_table_description as procedure(db_name varchar, schema_name varchar, table_name varchar)
    returns variant not null
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_table_description_py'
as $$
import json
def show_table_description_py(snowpark_session, db_name_py:str, schema_name_py:str, table_name_py:str):
    res = []
    
    table = snowpark_session.sql(f""SHOW TABLES like '{{table_name_py}}' IN SCHEMA {{db_name_py}}.{{schema_name_py}}"").collect()[0].as_dict()
    
    table['tags'] = []
    tag_query: str = f""SELECT * from table({DatabaseName}.INFORMATION_SCHEMA.TAG_REFERENCES('{DatabaseName}.{SchemaName}.{TableName}', 'TABLE'));""
    for tag in snowpark_session.sql(tag_query).to_local_iterator():
        table['tags'].append(tag.as_dict())
    
    for row in snowpark_session.sql(f'DESCRIBE TABLE {{db_name_py}}.{{schema_name_py}}.{{table_name_py}}').to_local_iterator():
        col = snowpark_session.sql(f""SHOW COLUMNS LIKE '{{row['name']}}' IN TABLE {{db_name_py}}.{{schema_name_py}}.{{table_name_py}}"").collect()[0].as_dict()
        res.append({{**row.as_dict(), **{{'tags': [], 'auto_increment': col['autoincrement'] if col['autoincrement'] != '' else None, 'data_type': json.loads(col['data_type'])}} }})
    
    column_tags_query:str = f""SELECT * from table({DatabaseName}.INFORMATION_SCHEMA.TAG_REFERENCES_ALL_COLUMNS('{DatabaseName}.{SchemaName}.{TableName}', 'TABLE'));""
    
    for column_tag in snowpark_session.sql(column_tags_query).to_local_iterator():
        next(col for col in res if col['name'] == column_tag['COLUMN_NAME'])['tags'].append(column_tag.as_dict())
    
    table['columns'] = res
    return table
$$
call show_table_description('{DatabaseName}', '{SchemaName}', '{TableName}')";
        return query;
    }

    public string GetObjectType()
    {
        return SnowflakeObject.Schema.ToSingularString();
    }

    public string GetObjectIdentifier()
    {
        return $"{DatabaseName}.{SchemaName}.{TableName}";
    }

    public bool IsProcedure()
    {
        return true;
    }
}
