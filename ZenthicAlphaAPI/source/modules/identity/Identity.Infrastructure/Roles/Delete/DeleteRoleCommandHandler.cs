using Application.Events;
using Application.Failures;
using Identity.Application.Roles.Delete;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Roles.Delete;

internal class DeleteRoleCommandHandler(
    IdentityModuleDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IRequestHandler<DeleteRoleCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var foundRole = await dbContext
            .Roles
            .FindAsync([command.Id], cancellationToken);

        if (foundRole is null)
            return FailureFactory.NotFound("Role not found", $"No role was found with an Id of {command.Id}");

        dbContext.Roles.Remove(foundRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new RoleDeletedEvent() { Entity = foundRole }
        );

        return new Success();
    }
}
