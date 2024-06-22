using Application._Common.Exceptions;
using Application._Common.Security.Authentication;
using Application._Common.Security.Authorization;
using Application.Users.ChangePassword;
using Domain.Security;
using MediatR;
using System.Reflection;

namespace Infrastructure._Common.Behaviors;

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
        if (IsAnonymousCall(request))
            return await next();

        var currentUser = await ValidateCurrentUserStatusAsync(request);
        ValidateAccessLevel(request, currentUser);

        return await next();
    }

    private static bool IsAnonymousCall(TRequest request)
    {
        var allowAnonymousAttribute = request
            .GetType()
            .GetCustomAttribute<AllowAnonymousAttribute>();

        return allowAnonymousAttribute is not null;
    }
    private async Task<ICurrentUser> ValidateCurrentUserStatusAsync(TRequest request)
    {
        if (identityService.IsCurrentUserNotAuthenticated())
            throw new UnauthorizedAccessException();

        var currentUser = await currentUserService
            .GetCurrentUserAsync()
        ?? throw new UnauthorizedAccessException();

        if (currentUser.Status is UserStatus.RequiredPasswordChange && request is not ChangeUserPasswordCommand)
            throw new PasswordChangeRequiredException();

        return currentUser;
    }
    private static void ValidateAccessLevel(TRequest request, ICurrentUser currentUser)
    {
        var authorizeAttribute = request
            .GetType()
            .GetCustomAttribute<AuthorizeAttribute>();

        if (authorizeAttribute is null)
            return;

        var (component, requiredAccess)
            = authorizeAttribute.GetData();

        var userAccess = currentUser
            .Accesses?
            .GetValueOrDefault(component, 0);

        if ((userAccess & requiredAccess) is 0)
            throw new ForbiddenAccessException();
    }
}
