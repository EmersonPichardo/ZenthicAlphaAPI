using Application.Auth;
using Application.Events;
using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Identity.Application.Users;
using Identity.Application.Users.GenerateEmailToken;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Settings;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;

namespace Identity.Infrastructure.Users.GenerateEmailToken;

internal class GenerateEmailTokenCommandHandler(
    IUserSessionInfo userSessionInfo,
    IdentityModuleDbContext dbContext,
    TokenManager tokenManager,
    IOptions<AuthSettings> authSettingsOptions,
    IEventPublisher eventPublisher
)
    : IRequestHandler<GenerateEmailTokenCommand, OneOf<string, Failure>>
{
    private readonly AuthSettings.TokenSettings tokenSettings = authSettingsOptions.Value.Token;

    public async Task<OneOf<string, Failure>> Handle(GenerateEmailTokenCommand command, CancellationToken cancellationToken)
    {
        var userId = command.UserId is not null
            ? command.UserId.Value
            : ((AuthenticatedSession)userSessionInfo.Session).Id;

        var foundUser = await dbContext
            .Users
            .Include(user => user.Tokens)
            .SingleOrDefaultAsync(
                user => user.Id.Equals(userId),
                cancellationToken
            );

        if (foundUser is null)
            return FailureFactory.UnauthorizedAccess();

        var foundUserToken = foundUser
            .Tokens
            .FirstOrDefault(
                userToken => userToken.Type.Equals(TokenType.EmailConfirmation)
            );

        var tokenResult = foundUserToken switch
        {
            null => await AddEmailTokenAsync(userId, cancellationToken),
            _ => UpdateEmailToken(foundUserToken)
        };

        if (tokenResult.IsFailure())
            return tokenResult;

        var token = tokenResult.GetValue<string>();

        foundUser.Status = foundUser.Status.AddFlag(UserStatus.UnconfirmEmail);

        dbContext.Update(foundUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new EmailTokenGeneratedEvent
            {
                User = UserDto.FromUser(foundUser),
                Token = token
            }
        );

        return token;
    }

    private async Task<OneOf<string, Failure>> AddEmailTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokenResult = tokenManager.Generate();

        var userToken = new UserToken
        {
            UserId = userId,
            Type = TokenType.EmailConfirmation,
            Token = tokenResult.HashedToken,
            HashingStamp = tokenResult.HashingStamp,
            Expiration = DateTime.UtcNow.Add(tokenSettings.LifeTime)
        };

        await dbContext.AddAsync(userToken, cancellationToken);

        return tokenResult.Token;
    }
    private OneOf<string, Failure> UpdateEmailToken(UserToken userToken)
    {
        var tokenResult = tokenManager.Generate();

        userToken.Token = tokenResult.HashedToken;
        userToken.HashingStamp = tokenResult.HashingStamp;
        userToken.Expiration = DateTime.UtcNow.Add(tokenSettings.LifeTime);

        dbContext.Update(userToken);

        return tokenResult.Token;
    }
}
