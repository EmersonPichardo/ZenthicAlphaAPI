namespace Application.Auth;

/// <summary>
/// Specifies that the class requires OAuth authentication.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeOAuthAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class AuthorizeOAuthAttribute
    : Attribute;
