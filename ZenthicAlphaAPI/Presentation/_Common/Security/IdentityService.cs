using Application._Common.Exceptions;
using Application._Common.Helpers;
using Application._Common.Security.Authentication;
using System.Security.Claims;

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

    public ICurrentUserIdentity? GetCurrentUserIdentity()
    {
        if (currentUserIdentity is not null)
            return currentUserIdentity;

        if (IsCurrentUserNotAuthenticated() && IsNotRefreshTokenCaller())
            return null;

        var claims = httpContextAccessor
            .HttpContext?
            .User
            .Claims;

        if (claims is null)
            return null;

        var idClaimValue = claims.GetByName(nameof(ICurrentUserIdentity.Id))
            ?? string.Empty;

        currentUserIdentity = Guid.TryParse(idClaimValue, out var id) ? id
            : throw new NotFoundException(nameof(Claim), idClaimValue);

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
