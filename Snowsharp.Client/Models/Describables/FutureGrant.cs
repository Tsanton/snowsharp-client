using Snowsharp.Client.Models.Assets;

namespace Snowsharp.Client.Models.Describables;

public class FutureGrant : ISnowflakeDescribable
{
    public FutureGrant(ISnowflakeGrantPrincipal principal)
    {
        Principal = principal;
    }

    public ISnowflakeGrantPrincipal Principal { get; init; }
    
    public string GetDescribeStatement()
    {
        string query;
        switch (Principal)
        {
            case Role principal:
                query = $@"
with show_grants_to_role as procedure(role_name varchar)
    returns variant not null
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_grants_to_role_py'
as $$
def show_grants_to_role_py(snowpark_session, role_name_py:str):
    res = []
    for row in snowpark_session.sql(f'SHOW FUTURE GRANTS TO ROLE {{role_name_py.upper()}}').to_local_iterator():
        res.append(row.as_dict())
    return res
$$
call show_grants_to_role('{principal.Name}');";
                return query;
            case DatabaseRole principal:
                query = $@"
with show_grants_to_database_role as procedure(database_name varchar, database_role_name varchar)
    returns variant not null
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_grants_to_database_role_py'
as $$
def show_grants_to_database_role_py(snowpark_session, database_name_py:str, database_role_name_py:str):
    res = []
    for row in snowpark_session.sql(f'SHOW FUTURE GRANTS IN DATABASE {{database_name_py.upper()}}').to_local_iterator():
        if row['grant_to'] == 'DATABASE_ROLE' and row['grant_to'] == 'DATABASE_ROLE':
                res.append(row.as_dict())
    for schema_object in snowpark_session.sql(f'SHOW SCHEMAS IN DATABASE {{database_name_py.upper()}}').to_local_iterator():
        schema_name:str = schema_object['name']
        if schema_name not in('INFORMATION_SCHEMA', 'PUBLIC'):
            query:str = f'SHOW FUTURE GRANTS IN SCHEMA {{database_name_py}}.{{schema_name}}'.upper()
            for row in snowpark_session.sql(query).to_local_iterator():
                if row['grant_to'] == 'DATABASE_ROLE' and row['grantee_name'] == database_role_name_py:
                    res.append(row.as_dict())
    return res
$$
call show_grants_to_database_role('{principal.DatabaseName}','{principal.Name}');";
                return query;
            default:
                throw new NotImplementedException("GetDescribeStatement is not implemented for this interface type");
        }
    }
}