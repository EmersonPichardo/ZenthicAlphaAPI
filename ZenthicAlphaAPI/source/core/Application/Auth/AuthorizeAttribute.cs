using Domain.Modularity;

namespace Application.Auth;

/// <summary>
/// Specifies that the class requires authorization.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class with a specific component and required access level.
/// </remarks>
/// <param name="component">Specifies the component of the class.</param>
/// <param name="permissions">Required access of the class.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class AuthorizeAttribute(
    Component component,
    params Permission[] permissions
)
    : Attribute
{
    private readonly Component component = component;
    private readonly Permission requiredAccess = permissions.Aggregate(
        (previousPermission, nextPermission) => previousPermission | nextPermission
    );

    public (Component component, Permission requiredAccess) GetData()
        => (component, requiredAccess);
}
