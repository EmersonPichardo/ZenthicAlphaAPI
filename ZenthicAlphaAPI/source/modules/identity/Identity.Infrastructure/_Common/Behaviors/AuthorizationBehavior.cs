using Application.Authorization;
using Application.Failures;
using Application.Helpers;
using Identity.Application._Common.Authentication;
using Identity.Application.Users.ChangePassword;
using Identity.Domain.User;
using MediatR;
using OneOf;
using OneOf.Types;
using System.Reflection;

namespace Identity.Infrastructure._Common.Behaviors;

internal class AuthorizationBehavior<TRequest, TResponse>(
    IIdentityService identityService,
    ICurrentUserService currentUserService
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await next().ConfigureAwait(false);

        if (IsAnonymousCall(request))
            return await next().ConfigureAwait(false);

        var validCurrentUserResult = await GetValidCurrentUserAsync(request);

        if (validCurrentUserResult.IsFailure())
            return validCurrentUserResult.GetValueAs<dynamic>();

        var currentUser = validCurrentUserResult.GetValueAs<ICurrentUser>();

        var validateAccessLevelResult = ValidateAccessLevel(currentUser, request);

        if (validateAccessLevelResult.IsFailure())
            return validateAccessLevelResult.GetValueAs<dynamic>();

        return await next().ConfigureAwait(false);
    }

    private static bool IsAnonymousCall(TRequest request)
    {
        var allowAnonymousAttribute = request
            .GetType()
            .GetCustomAttribute<AllowAnonymousAttribute>();

        return allowAnonymousAttribute is not null;
    }
    private async Task<OneOf<ICurrentUser, Failure>> GetValidCurrentUserAsync(TRequest request)
    {
        if (identityService.IsCurrentUserNotAuthenticated())
            return FailureFactory.UnauthorizedAccess("", "");

        var currentUserResult = await currentUserService
            .GetCurrentUserAsync();

        return currentUserResult.Match(
            currentUser => AuthorizationBehavior<TRequest, TResponse>.ValidateCurrentUser(currentUser, request).Match(
                none => OneOf<ICurrentUser, Failure>.FromT0(currentUser),
                failure => failure
            ),
            none => FailureFactory.UnauthorizedAccess("", ""),
            failure => failure
        );
    }
    private static OneOf<None, Failure> ValidateCurrentUser(ICurrentUser currentUser, TRequest request)
    {
        if (currentUser.Status is UserStatus.RequiredPasswordChange && request is not ChangeUserPasswordCommand)
            return FailureFactory.PasswordChangeRequired("", "");

        return new None();
    }
    private static OneOf<None, Failure> ValidateAccessLevel(ICurrentUser currentUser, TRequest request)
    {
        var authorizeAttribute = request
            .GetType()
            .GetCustomAttribute<AuthorizeAttribute>();

        if (authorizeAttribute is null)
            return new None();

        var (component, requiredAccess)
            = authorizeAttribute.GetData();

        var userAccess = currentUser
            .Accesses?
            .GetValueOrDefault(component, 0);

        if ((userAccess & requiredAccess) is 0)
            return FailureFactory.ForbiddenAccess("", "");

        return new None();
    }
}
