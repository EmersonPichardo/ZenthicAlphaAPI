using Application.Auth;
using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Domain.Modularity;
using Identity.Domain.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Security.Claims;
using System.Security.Principal;
using IUserSession = Application.Auth.IUserSession;

namespace Identity.Infrastructure.Common.Auth;

internal class UserSessionSession(
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserSessionSession>? logger
)
    : IUserSessionService
{
    private IUserSession? session;

    public async Task<IUserSession> GetSessionAsync()
    {
        try
        {
            if (session is not null) return session;

            var httpContext = httpContextAccessor.HttpContext;
            var claimsPrincipal = httpContext?.User;
            var identity = claimsPrincipal?.Identity;

            if (httpContext is null || claimsPrincipal is null || identity is null)
                return session = new AnonymousSession();

            var (isOAuthCall, authenticationResult) = await IsOAuthCallAsync(httpContext);
            if (isOAuthCall) return session = NewOAuthSession(httpContext, authenticationResult, logger);

            if (IsAnonymousCall(identity))
                return session = new AnonymousSession();

            if (IsRefreshTokenCall(claimsPrincipal))
                return session = NewRefreshTokenSession(claimsPrincipal, logger);

            return session = NewAuthorizedSession(claimsPrincipal, logger);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "Sesion inválida");
            return new AnonymousSession();
        }

        static bool IsAnonymousCall(IIdentity identity)
        {
            return !identity.IsAuthenticated;
        }
        static async Task<(bool, AuthenticateResult authenticationResult)> IsOAuthCallAsync(HttpContext httpContext)
        {
            if (!httpContext.Request.Query.TryGetValue("authenticationScheme", out var authenticationScheme))
                return (false, AuthenticateResult.Fail("No authentication scheme found"));

            var authenticationResult = await httpContext.AuthenticateAsync(authenticationScheme);

            return (authenticationResult.Succeeded, authenticationResult);
        }
        static bool IsRefreshTokenCall(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.HasClaim(JwtManager.RefreshTokenIdentifier);
        }
    }

    private static IUserSession NewOAuthSession(
        HttpContext httpContext, AuthenticateResult authenticationResult, ILogger<UserSessionSession>? logger)
    {
        var claimsPrincipal = authenticationResult.Principal!;

        var authenticationType = claimsPrincipal.Identity?.AuthenticationType ?? OAuthDefaults.DisplayName;

        var userNameClaimResult = claimsPrincipal.GetStringByName(nameof(AuthenticatedSession.UserName));
        if (userNameClaimResult.IsFailure()) return HandleFailureResult(userNameClaimResult, logger);
        var userName = userNameClaimResult.GetValue<string>();

        var emailClaimResult = claimsPrincipal.GetStringByName(nameof(AuthenticatedSession.Email));
        if (emailClaimResult.IsFailure()) return HandleFailureResult(emailClaimResult, logger);
        var email = emailClaimResult.GetValue<string>();

        var statusClaimResult = claimsPrincipal.GetEnumByName<OAuthUserStatus>(nameof(AuthenticatedSession.Status));
        if (statusClaimResult.IsFailure()) return HandleFailureResult(statusClaimResult, logger);
        var status = statusClaimResult.GetValue<OAuthUserStatus>();

        var redirectUrl = authenticationResult.Properties?.Items["redirectUrl"];
        if (redirectUrl is null) return HandleFailureResult(FailureFactory.Generic("No redirect url found"), logger);

        httpContext.Response.Cookies.Delete(
            CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme
        );

        return new OAuthSession
        {
            AuthenticationType = authenticationType,
            UserName = userName,
            Email = email,
            Status = status,
            RedirectUrl = redirectUrl,
        };
    }
    private static IUserSession NewAuthorizedSession(
        ClaimsPrincipal claimsPrincipal, ILogger<UserSessionSession>? logger)
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
    private static IUserSession NewRefreshTokenSession(
        ClaimsPrincipal claimsPrincipal, ILogger<UserSessionSession>? logger)
    {
        var userIdClaimResult = claimsPrincipal.GetGuidByName(nameof(RefreshTokenSession.UserId));
        if (userIdClaimResult.IsFailure()) return HandleFailureResult(userIdClaimResult, logger);

        return new RefreshTokenSession
        {
            UserId = userIdClaimResult.GetValue<Guid>()
        };
    }

    private static AnonymousSession HandleFailureResult(
        IOneOf result, ILogger<UserSessionSession>? logger)
    {
        var exception = result.GetFailure().ToException();
        logger?.LogError(exception, "Sesion inválida");

        return new AnonymousSession();
    }
    private static AnonymousSession HandleFailureResult(
        Failure failure, ILogger<UserSessionSession>? logger)
    {
        var exception = failure.ToException();
        logger?.LogError(exception, "Sesion inválida");

        return new AnonymousSession();
    }
}
