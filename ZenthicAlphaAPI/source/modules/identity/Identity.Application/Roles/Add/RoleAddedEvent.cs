using Application.Events;
using Identity.Domain.Roles;

namespace Identity.Application.Roles.Add;

public record RoleAddedEvent
    : BaseEntityEvent<Role>;
