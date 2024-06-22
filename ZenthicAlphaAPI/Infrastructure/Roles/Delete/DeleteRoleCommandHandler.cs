using Application._Common.Events;
using Application._Common.Exceptions;
using Application._Common.Persistence.Databases;
using Application.Roles.Delete;

namespace Infrastructure.Roles.Delete;

internal class DeleteRoleCommandHandler(
    IApplicationDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IDeleteRoleCommandHandler
{
    public async Task Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var foundRole = await dbContext
            .Roles
            .FindAsync([command.Id], cancellationToken)
        ?? throw new NotFoundException(nameof(dbContext.Roles), command.Id);

        dbContext.Roles.Remove(foundRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new RoleDeletedEvent() { Entity = foundRole }
        );
    }
}
