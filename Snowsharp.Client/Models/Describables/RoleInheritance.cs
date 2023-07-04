using System.Data;
using Snowsharp.Client.Models.Commons;
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;

/// <summary>
/// RoleInheritance is the action of granting USAGE on a (database) role to a principal (user, role or database role).
/// </summary>
public class RoleInheritance : ISnowflakeDescribable
{
    public RoleInheritance(ISnowflakePrincipal inheritedRole, ISnowflakePrincipal parentPrincipal)
    {
        InheritedRole = inheritedRole;
        ParentPrincipal = parentPrincipal;
    }

    public ISnowflakePrincipal InheritedRole { get; set; }
    public ISnowflakePrincipal ParentPrincipal { get; set; }
    public string GetDescribeStatement()
    {
        string inheritedRoleType;
        string inheritedRoleIdentifier;
        switch (InheritedRole.GetObjectType())
        {
            case "ROLE":
                inheritedRoleType = "ROLE";
                inheritedRoleIdentifier = InheritedRole.GetObjectIdentifier();
                break;
            case "DATABASE_ROLE":
                inheritedRoleType = "DATABASE_ROLE";
                inheritedRoleIdentifier = InheritedRole.GetObjectIdentifier();
                break;
            default:
                throw new NotImplementedException("GetDescribeStatement is not implemented for this interface type");
        }
        string parentPrincipalType;
        string parentPrincipalIdentifier;
        switch (ParentPrincipal.GetObjectType())
        {
            case "ROLE":
                parentPrincipalType = "ROLE";
                parentPrincipalIdentifier = ParentPrincipal.GetObjectIdentifier();
                break;
            case "DATABASE_ROLE":
                parentPrincipalType = "DATABASE ROLE";
                parentPrincipalIdentifier = ParentPrincipal.GetObjectIdentifier();
                break;
            default:
                throw new NotImplementedException("GetDescribeStatement is not implemented for this interface type");
        }
        if (parentPrincipalType == SnowflakePrincipal.DatabaseRole.GetSnowflakeType() && inheritedRoleType == SnowflakePrincipal.Role.GetSnowflakeType())
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
call show_inherited_role('{parentPrincipalIdentifier}', '{parentPrincipalType}', '{inheritedRoleIdentifier}', '{inheritedRoleType}');";
        return query;
    }

    public bool IsProcedure()
    {
        return true;
    }
}