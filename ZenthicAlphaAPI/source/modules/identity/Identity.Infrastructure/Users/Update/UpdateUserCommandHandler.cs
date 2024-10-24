using Application.Auth;
using Application.Failures;
using Identity.Application.Users.GenerateEmailToken;
using Identity.Application.Users.Update;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.Update;

internal class UpdateUserCommandHandler(
    IdentityModuleDbContext dbContext,
    IUserSessionService userSessionService,
    ISender sender
)
    : IRequestHandler<UpdateUserCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var userSession = await userSessionService.GetSessionAsync();
        var authenticatedSession = (AuthenticatedSession)userSession;

        var foundUser = await dbContext
            .Users
            .FindAsync([authenticatedSession.Id], cancellationToken);

        if (foundUser is null)
            return FailureFactory.NotFound("User not found", $"User not found by Id {authenticatedSession.Id}");

        if (foundUser.UserName == command.UserName && foundUser.Email == command.Email)
            return new Success();

        if (foundUser.UserName != command.UserName)
            foundUser.UserName = command.UserName;

        if (foundUser.Email != command.Email)
        {
            foundUser.Email = command.Email;

            var generateEmailTokenCommand = new GenerateEmailTokenCommand { UserId = authenticatedSession.Id };
            await sender.Send(generateEmailTokenCommand, cancellationToken);
        }

        dbContext.Update(foundUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
