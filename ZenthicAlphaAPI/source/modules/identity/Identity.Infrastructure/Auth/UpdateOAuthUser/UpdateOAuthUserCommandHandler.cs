using Application.Failures;
using Identity.Application.Auth.UpdateOAuthUser;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Auth.UpdateOAuthUser;

internal class UpdateOAuthUserCommandHandler(
    IdentityModuleDbContext dbContext
)
    : IRequestHandler<UpdateOAuthUserCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(UpdateOAuthUserCommand command, CancellationToken cancellationToken)
    {
        var foundOAuthUser = await dbContext
            .OAuthUsers
            .FindAsync([command.Id], cancellationToken);

        if (foundOAuthUser is null)
            return FailureFactory.NotFound("OAuth user not found", $"OAuth user not found by Id {command.Id}");

        if (foundOAuthUser.UserName == command.UserName && foundOAuthUser.Email == command.Email)
            return new Success();

        if (foundOAuthUser.UserName != command.UserName)
            foundOAuthUser.UserName = command.UserName;

        if (foundOAuthUser.Email != command.Email)
            foundOAuthUser.Email = command.Email;

        dbContext.Update(foundOAuthUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
