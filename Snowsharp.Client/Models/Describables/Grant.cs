using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;


public class Grant : ISnowflakeDescribable
{
    public Grant(ISnowflakeGrantPrincipal principal)
    {
        Principal = principal;
    }

    public ISnowflakeGrantPrincipal Principal { get; init; }
    
    public string GetDescribeStatement()
    {
        string principalType;
        string principalIdentifier;
        switch (Principal)
        {
            case Role principal:
                principalType = SnowflakePrincipal.Role.GetSnowflakeType();
                principalIdentifier = principal.Name;
                break;
            case DatabaseRole principal:
                principalType = SnowflakePrincipal.DatabaseRole.GetSnowflakeType();
                principalIdentifier = $"{principal.DatabaseName}.{principal.Name}";
                break;
            default:
                throw new NotImplementedException("GetDescribeStatement is not implemented for this interface type");
        }
            var query = $@"
with show_grants_to_principal as procedure(principal_type varchar, principal_identifier varchar)
    returns variant not null
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_grants_to_principal_py'
as $$
def show_grants_to_principal_py(snowpark_session, principal_type_py:str, principal_identifier_py:str):
    res = []
    for row in snowpark_session.sql(f'SHOW GRANTS TO {{principal_type_py}} {{principal_identifier_py}}').to_local_iterator():
        res.append(row.as_dict())
    return res
$$
call show_grants_to_principal('{principalType}', '{principalIdentifier}');";
                return query;
    }
}