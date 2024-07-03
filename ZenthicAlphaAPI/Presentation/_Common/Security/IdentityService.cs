using Application._Common.Failures;
using Application._Common.Helpers;
using Application._Common.Security.Authentication;
using OneOf;
using OneOf.Types;

namespace Presentation._Common.Security;

internal class IdentityService(
    IHttpContextAccessor httpContextAccessor
)
    : IIdentityService
{
    private bool? isAuthenticated;
    private CurrentUserIdentity? currentUserIdentity;
    private bool? isRefreshToken;

    public bool IsCurrentUserAuthenticated()
    {
        return isAuthenticated
            ??= (httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false) && IsNotRefreshTokenCaller();
    }

    public OneOf<ICurrentUserIdentity, None, Failure> GetCurrentUserIdentity()
    {
        if (currentUserIdentity is not null)
            return currentUserIdentity;

        if (IsCurrentUserNotAuthenticated() && IsNotRefreshTokenCaller())
            return new None();

        var claims = httpContextAccessor
            .HttpContext?
            .User
            .Claims;

        if (claims is null)
            return new None();

        var idClaimValue = claims.GetByName(nameof(ICurrentUserIdentity.Id))
            ?? string.Empty;

        currentUserIdentity = Guid.TryParse(idClaimValue, out var id) ? id : null;

        if (currentUserIdentity is null)
            return FailureFactory.NotFound("Sesión inválida", "No se encontró el Id de la sesión");

        return currentUserIdentity;
    }

    public bool IsCurrentUserNotAuthenticated()
        => !IsCurrentUserAuthenticated();

    public bool IsRefreshTokenCaller()
    {
        if (isRefreshToken.HasValue)
            return isRefreshToken.Value;

        var isRefreshTokenClaimValue = httpContextAccessor
            .HttpContext?
            .User?
            .Claims
            .GetByName("IsRefreshToken");

        isRefreshToken = bool.TryParse(isRefreshTokenClaimValue, out var isRefreshTokenValue)
            && isRefreshTokenValue;

        return isRefreshToken.Value;
    }

    public bool IsNotRefreshTokenCaller()
        => !IsRefreshTokenCaller();
}
