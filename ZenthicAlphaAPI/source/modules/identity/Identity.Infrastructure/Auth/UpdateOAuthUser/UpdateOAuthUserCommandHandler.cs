using Application.Auth;
using Application.Failures;
using Identity.Application.Auth.UpdateOAuthUser;
using Identity.Application.Common.Auth;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Auth.UpdateOAuthUser;

internal class UpdateOAuthUserCommandHandler(
    IUserSessionService userSessionService,
    IdentityModuleDbContext dbContext
)
    : IRequestHandler<UpdateOAuthUserCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(UpdateOAuthUserCommand command, CancellationToken cancellationToken)
    {
        var userSession = await userSessionService.GetSessionAsync();
        var oauthSession = (OAuthSession)userSession;

        var foundOAuthUser = await dbContext
            .OAuthUsers
            .FirstOrDefaultAsync(
                oauthUser
                    => oauthUser.AuthenticationType == oauthSession.AuthenticationType
                    && oauthUser.Email == oauthSession.Email,
                cancellationToken
            );

        if (foundOAuthUser is null)
            return FailureFactory.NotFound(
                "OAuth user not found",
                $"OAuth user not found by AuthenticationType {oauthSession.AuthenticationType} and Email {oauthSession.Email}"
            );

        if (foundOAuthUser.UserName == oauthSession.UserName && foundOAuthUser.Email == oauthSession.Email)
            return new Success();

        if (foundOAuthUser.UserName != oauthSession.UserName)
            foundOAuthUser.UserName = oauthSession.UserName;

        if (foundOAuthUser.Email != oauthSession.Email)
            foundOAuthUser.Email = oauthSession.Email;

        dbContext.Update(foundOAuthUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
