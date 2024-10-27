namespace Identity.Application.Auth;

/// <summary>
/// Specifies that the class requires OAuth authentication.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeRefreshTokenAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class AuthorizeRefreshTokenAttribute
    : Attribute;
