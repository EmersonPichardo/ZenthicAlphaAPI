﻿using Domain.Entities.Implementations;
using Identity.Domain.Roles;

namespace Identity.Domain.User;

public class UserRole : BaseAuditableCompoundEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public User? User { get; set; }
    public Role? Role { get; set; }
}