using Application.Auth;
using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Identity.Application.Users.ConfirmEmail;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.ConfirmEmail;

internal class ConfirmEmailCommandHandler(
    IdentityModuleDbContext dbContext,
    IUserSessionInfo userSessionInfo,
    TokenManager tokenManager
)
    : IRequestHandler<ConfirmEmailCommand, OneOf<Success, Failure>>
{
    private readonly AuthenticatedSession authenticatedSession = (AuthenticatedSession)userSessionInfo.Session;

    public async Task<OneOf<Success, Failure>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .Include(user => user.Tokens)
            .SingleOrDefaultAsync(
                user => user.Id.Equals(authenticatedSession.Id),
                cancellationToken
            );

        if (foundUser is null)
            return FailureFactory.UnauthorizedAccess();

        if (foundUser.Status.NotHasFlag(UserStatus.UnconfirmEmail))
            return FailureFactory.InvalidRequest("Email already confirmed");

        var foundUserToken = foundUser
            .Tokens
            .FirstOrDefault(userToken => userToken.Type.Equals(TokenType.EmailConfirmation));

        if (foundUserToken is null)
            return FailureFactory.NotFound(
                "Token not found",
                $"Token of type {TokenType.EmailConfirmation} for user {foundUser.UserName} was not found"
            );

        if (foundUserToken.Expiration < DateTime.UtcNow)
            return FailureFactory.InvalidRequest("Invalid token", "The token is expired");

        var areTokensEquals = tokenManager.Equals(
            command.Token, foundUserToken.Token, foundUserToken.HashingStamp
        );

        if (!areTokensEquals)
            return FailureFactory.InvalidRequest("Invalid token");

        foundUser.Status = foundUser.Status.RemoveFlag(UserStatus.UnconfirmEmail);

        dbContext.Update(foundUser);
        dbContext.Remove(foundUserToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
