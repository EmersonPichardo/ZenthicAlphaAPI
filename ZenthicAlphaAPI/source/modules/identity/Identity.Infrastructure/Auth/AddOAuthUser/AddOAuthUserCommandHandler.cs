using Application.Auth;
using Application.Failures;
using Identity.Application.Auth.AddOAuthUser;
using Identity.Application.Common.Auth;
using Identity.Domain.User;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;

namespace Identity.Infrastructure.Auth.AddOAuthUser;

internal class AddOAuthUserCommandHandler(
    IUserSessionService userSessionService,
    IdentityModuleDbContext dbContext
)
    : IRequestHandler<AddOAuthUserCommand, OneOf<AddOAuthUserCommandResponse, Failure>>
{
    public async Task<OneOf<AddOAuthUserCommandResponse, Failure>> Handle(AddOAuthUserCommand request, CancellationToken cancellationToken)
    {
        var userSession = await userSessionService.GetSessionAsync();
        var oauthSession = (OAuthSession)userSession;

        var newOAuthUser = new OAuthUser
        {
            AuthenticationType = oauthSession.AuthenticationType,
            UserName = oauthSession.UserName,
            Email = oauthSession.Email,
            Status = OAuthUserStatus.Active
        };

        await dbContext.OAuthUsers.AddAsync(newOAuthUser, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddOAuthUserCommandResponse
        {
            Id = newOAuthUser.Id,
            UserName = newOAuthUser.UserName,
            Email = newOAuthUser.Email,
            Status = newOAuthUser.Status.ToString()
        };
    }
}
