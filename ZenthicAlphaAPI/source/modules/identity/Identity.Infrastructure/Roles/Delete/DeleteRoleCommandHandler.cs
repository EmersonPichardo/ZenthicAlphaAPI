using Application.Events;
using Application.Failures;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.Delete;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Roles.Delete;

internal class DeleteRoleCommandHandler(
    IIdentityDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IRequestHandler<DeleteRoleCommand, OneOf<None, Failure>>
{
    public async Task<OneOf<None, Failure>> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
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

        return new None();
    }
}
