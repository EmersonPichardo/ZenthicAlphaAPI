using Application._Common.Events;
using Application._Common.Failures;
using Application._Common.Persistence.Databases;
using Application.Roles.Delete;
using OneOf;
using OneOf.Types;

namespace Infrastructure.Roles.Delete;

internal class DeleteRoleCommandHandler(
    IApplicationDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IDeleteRoleCommandHandler
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
