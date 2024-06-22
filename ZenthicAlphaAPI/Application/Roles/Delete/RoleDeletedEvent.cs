using Domain.Security;

namespace Application.Roles.Delete;

public record RoleDeletedEvent
    : BaseEntityEvent<Role>;
