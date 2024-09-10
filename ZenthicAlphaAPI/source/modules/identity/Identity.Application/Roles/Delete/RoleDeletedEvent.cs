using Application.Events;
using Identity.Domain.Roles;

namespace Identity.Application.Roles.Delete;

public record RoleDeletedEvent
    : BaseEntityEvent<Role>;
