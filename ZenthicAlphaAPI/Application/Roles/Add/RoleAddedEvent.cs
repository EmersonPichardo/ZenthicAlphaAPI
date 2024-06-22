using Domain.Security;

namespace Application.Roles.Add;

public record RoleAddedEvent
    : BaseEntityEvent<Role>;
