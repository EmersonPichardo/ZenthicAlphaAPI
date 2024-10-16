using Application.Auth;
using Application.Helpers;
using Domain.Identity;
using Domain.Modularity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Security.Claims;
using IUserSession = Application.Auth.IUserSession;

namespace Identity.Infrastructure.Common.Auth;

internal class UserSessionInfo(
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserSessionInfo>? logger
)
    : IUserSessionInfo
{
    public IUserSession Session { get; init; } = BuildSession(httpContextAccessor, logger);

    private static IUserSession BuildSession(IHttpContextAccessor httpContextAccessor, ILogger<UserSessionInfo>? logger)
    {
        try
        {
            var claimsPrincipal = httpContextAccessor
                .HttpContext?
                .User;

            if (claimsPrincipal is null || IsAnonymousCall(claimsPrincipal))
                return new AnonymousSession();

            if (IsRefreshTokenCall(claimsPrincipal))
                return NewRefreshTokenSession(claimsPrincipal, logger);

            return NewAuthorizedSession(claimsPrincipal, logger);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Sesion inválida");
            return new AnonymousSession();
        }

        static bool IsAnonymousCall(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Identity?.IsAuthenticated is not true;
        }
        static bool IsRefreshTokenCall(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.HasClaim(JwtManager.RefreshTokenIdentifier);
        }
    }
    private static IUserSession NewAuthorizedSession(ClaimsPrincipal claimsPrincipal, ILogger<UserSessionInfo>? logger)
    {
        var idClaimResult = claimsPrincipal.GetGuidByName(nameof(AuthenticatedSession.Id));
        if (idClaimResult.IsFailure()) return HandleFailureResult(idClaimResult, logger);
        var id = idClaimResult.GetValue<Guid>();

        var userNameClaimResult = claimsPrincipal.GetStringByName(nameof(AuthenticatedSession.UserName));
        if (userNameClaimResult.IsFailure()) return HandleFailureResult(userNameClaimResult, logger);
        var userName = userNameClaimResult.GetValue<string>();

        var emailClaimResult = claimsPrincipal.GetStringByName(nameof(AuthenticatedSession.Email));
        if (emailClaimResult.IsFailure()) return HandleFailureResult(emailClaimResult, logger);
        var email = emailClaimResult.GetValue<string>();

        var statusClaimResult = claimsPrincipal.GetEnumByName<UserStatus>(nameof(AuthenticatedSession.Status));
        if (statusClaimResult.IsFailure()) return HandleFailureResult(statusClaimResult, logger);
        var status = statusClaimResult.GetValue<UserStatus>();

        var accessesClaimResult = claimsPrincipal.GetJsonByName<Dictionary<Component, Permission>>(nameof(AuthorizedSession.Accesses));
        if (accessesClaimResult.IsFailure()) return HandleFailureResult(accessesClaimResult, logger);
        var accesses = accessesClaimResult.GetValue<Dictionary<Component, Permission>>();

        return new AuthorizedSession
        {
            Id = id,
            UserName = userName,
            Email = email,
            Status = status,
            Accesses = accesses
        };
    }
    private static IUserSession NewRefreshTokenSession(ClaimsPrincipal claimsPrincipal, ILogger<UserSessionInfo>? logger)
    {
        var userIdClaimResult = claimsPrincipal.GetGuidByName(nameof(RefreshTokenSession.UserId));
        if (userIdClaimResult.IsFailure()) return HandleFailureResult(userIdClaimResult, logger);

        return new RefreshTokenSession
        {
            UserId = userIdClaimResult.GetValue<Guid>()
        };
    }
    private static AnonymousSession HandleFailureResult(IOneOf result, ILogger<UserSessionInfo>? logger)
    {
        var exception = result.GetFailure().ToException();
        logger?.LogError(exception, "Sesion inválida");

        return new AnonymousSession();
    }
}
