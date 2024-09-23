namespace Application.Authorization;

/// <summary>
/// Specifies that the class doesn't requires authentication.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AllowAnonymousAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class AllowAnonymousAttribute
    : Attribute;
