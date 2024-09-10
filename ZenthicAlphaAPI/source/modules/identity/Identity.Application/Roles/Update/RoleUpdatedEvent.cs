using Application.Events;
using Identity.Domain.Roles;

namespace Identity.Application.Roles.Update;

public record RoleUpdatedEvent
    : BaseEntityEvent<Role>;