using System.Data;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;

/// <summary>
/// RoleInheritance is the action of granting USAGE on a (database) role to a principal (user, role or database role).
/// </summary>
public class RoleInheritance: ISnowflakeDescribable
{
    public RoleInheritance(ISnowflakeGrantPrincipal inheritedRole, ISnowflakeGrantPrincipal parentPrincipal)
    {
        InheritedRole = inheritedRole;
        ParentPrincipal = parentPrincipal;
    }

    public ISnowflakeGrantPrincipal InheritedRole { get; set; }
    public ISnowflakeGrantPrincipal ParentPrincipal { get; set; }
    public string GetDescribeStatement()
    {
        SnowflakePrincipal inheritedRoleType;
        string inheritedRoleIdentifier;
        switch (InheritedRole)
        {
            case Role role:
                inheritedRoleType = SnowflakePrincipal.Role;
                inheritedRoleIdentifier = role.Name;
                break;
            case DatabaseRole role:
                inheritedRoleType = SnowflakePrincipal.DatabaseRole;
                inheritedRoleIdentifier = $"{role.DatabaseName}.{role.Name}";
                break;
            default:
                throw new NotImplementedException("GetDescribeStatement is not implemented for this interface type");
        }
        SnowflakePrincipal parentPrincipalType;
        string parentPrincipalIdentifier;
        switch (ParentPrincipal)
        {
            case Role principal:
                parentPrincipalType = SnowflakePrincipal.Role;
                parentPrincipalIdentifier = principal.Name;
                break;
            case DatabaseRole principal:
                parentPrincipalType = SnowflakePrincipal.DatabaseRole;
                parentPrincipalIdentifier = $"{principal.DatabaseName}.{principal.Name}";
                break;
            default:
                throw new NotImplementedException("GetDescribeStatement is not implemented for this interface type");
        }
        if (parentPrincipalType == SnowflakePrincipal.DatabaseRole && inheritedRoleType == SnowflakePrincipal.Role)
        {
            throw new ConstraintException("Account roles cannot be granted to database roles");
        }
        var query = $@"
with show_inherited_role as procedure(parent_principal_identifier varchar, parent_principal_type varchar, child_role_identifier varchar, child_role_type varchar)
    returns variant
    language python
    runtime_version = '3.8'
    packages = ('snowflake-snowpark-python')
    handler = 'show_inherited_role_py'
as $$
def show_inherited_role_py(snowpark_session, parent_principal_identifier_py:str, parent_principal_type_py:str, child_role_identifier_py:str, child_role_type_py:str):
    for row in snowpark_session.sql(f'SHOW GRANTS TO {{parent_principal_type_py}} {{parent_principal_identifier_py}}'.upper()).to_local_iterator():
        if row['granted_on'] == child_role_type_py and row['name'] == child_role_identifier_py and row['privilege'] == 'USAGE':
            return row.as_dict()
    raise ValueError('Role relationship does not exist or not authorized')
$$
call show_inherited_role('{parentPrincipalIdentifier}', '{parentPrincipalType.GetSnowflakeType()}', '{inheritedRoleIdentifier}', '{inheritedRoleType.GetEnumJsonAttributeValue()}');";
        return query;
    }

    public bool IsProcedure()
    {
        return true;
    }
}