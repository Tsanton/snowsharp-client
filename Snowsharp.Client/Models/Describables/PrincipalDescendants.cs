using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;

/// <summary>
/// Get direct children (inherited roles and database roles) for any snowflake principal. Only one level removed
/// </summary>
public class PrincipalDescendants: ISnowflakeDescribable
{
    public PrincipalDescendants(ISnowflakeGrantPrincipal principal)
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
                throw new NotImplementedException();
        }
        var query = $@"
with show_direct_descendants_from_principal as procedure(principal_type varchar, principal_identifier varchar)
    returns variant not null
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_direct_descendants_from_principal_py'
as $$
def show_direct_descendants_from_principal_py(snowpark_session, principal_type_py:str, principal_identifier_py:str):
    res = []
    for row in snowpark_session.sql(f'SHOW GRANTS TO {{principal_type_py}} {{principal_identifier_py}}').to_local_iterator():
        if row['privilege'] == 'USAGE' and row['granted_on'] in ['ROLE', 'DATABASE_ROLE']:
            res.append({{ 
                **row.as_dict(), 
                **{{'distance_from_source': 0 }} 
            }})
    return {{
        'principal_identifier': principal_identifier_py,
        'principal_type': principal_type_py if principal_type_py != 'DATABASE ROLE' else 'DATABASE_ROLE',
        'descendants': res
    }}
$$
call show_direct_descendants_from_principal('{principalType}', '{principalIdentifier}');";
        return query; 
    }
}