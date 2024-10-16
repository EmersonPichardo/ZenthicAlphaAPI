using Application.Auth;
using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Domain.Modularity;
using Identity.Application.Users.ChangePassword;
using Identity.Application.Users.RefreshToken;
using Identity.Infrastructure.Common.Auth;
using Infrastructure.Behaviors;
using OneOf;
using OneOf.Types;
using System.Reflection;

namespace Identity.Infrastructure.Common.ModuleBehaviors;

internal class AuthorizationModuleBehavior(
    IUserSessionInfo userSessionInfo
)
    : IModuleBehavior
{
    public async Task<OneOf<Success, Failure>> HandleAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken
    )
        where TRequest : notnull
    {
        if (IsAnonymousCallAllow(request))
            return new Success();

        var result = userSessionInfo.Session switch
        {
            AnonymousSession => FailureFactory.UnauthorizedAccess(),
            RefreshTokenSession => ValidateRefreshTokenSession(request),
            AuthorizedSession authorizedSession => ValidateAuthorizedSession(request, authorizedSession).Match(
                success => ValidateAccessLevel(authorizedSession, request),
                failure => failure
            ),
            _ => FailureFactory.Generic("Unhandled session type", $"The session type {userSessionInfo.Session} doesn't has a handler")
        };

        return await Task.FromResult(result);
    }

    private static bool IsAnonymousCallAllow<TRequest>(TRequest request)
        where TRequest : notnull
    {
        var allowAnonymousAttribute = request
            .GetType()
            .GetCustomAttribute<AllowAnonymousAttribute>();

        return allowAnonymousAttribute is not null;
    }
    private static OneOf<Success, Failure> ValidateRefreshTokenSession<TRequest>(TRequest request)
    {
        if (request is not RefreshUserTokenCommand)
            return FailureFactory.UnauthorizedAccess();

        return new Success();
    }
    private static OneOf<Success, Failure> ValidateAuthorizedSession<TRequest>(TRequest request, AuthorizedSession authorizedSession)
    {
        var isPasswordChangeRequired
            = authorizedSession.Status.HasFlag(UserStatus.PasswordChangeRequired)
            && request is not ChangeUserPasswordCommand;

        if (isPasswordChangeRequired)
            return FailureFactory.PasswordChangeRequired();

        return new Success();
    }
    private static OneOf<Success, Failure> ValidateAccessLevel<TRequest>(AuthorizedSession authorizedSession, TRequest request)
        where TRequest : notnull
    {
        var authorizeAttributes = request
            .GetType()
            .GetCustomAttributes<AuthorizeAttribute>();

        foreach (var authorizeAttribute in authorizeAttributes)
        {
            var (component, requiredAccess) = authorizeAttribute.GetData();

            var userAccess = authorizedSession
                .Accesses
                .GetValueOrDefault(component, Permission.None);

            if (userAccess.NotHasFlag(requiredAccess))
                return FailureFactory.ForbiddenAccess();
        }

        return new Success();
    }
}
