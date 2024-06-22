using Domain._Common.Modularity;

namespace Application._Common.Security.Authorization;

/// <summary>
/// Specifies that the class requires authorization.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class with a specific component and required access level.
/// </remarks>
/// <param name="component">Specifies the component of the class.</param>
/// <param name="requiredAccess">Required access of the class.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class AuthorizeAttribute(
    Component component,
    Permission requiredAccess
)
    : Attribute
{
    private readonly Component component = component;
    private readonly int requiredAccess = (int)requiredAccess;

    public (Component component, int requiredAccess) GetData()
        => (component, requiredAccess);
}
