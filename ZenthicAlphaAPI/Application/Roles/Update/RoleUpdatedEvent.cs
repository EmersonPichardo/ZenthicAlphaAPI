using Domain.Security;

namespace Application.Roles.Update;

public record RoleUpdatedEvent
    : BaseEntityEvent<Role>;