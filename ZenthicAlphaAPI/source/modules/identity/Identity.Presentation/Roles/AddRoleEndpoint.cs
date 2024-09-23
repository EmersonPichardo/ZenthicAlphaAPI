﻿using Domain.Modularity;
using Identity.Application.Roles.Add;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record AddRoleEndpoint : DefaultAddEndpoint<AddRoleCommand>
{
    public AddRoleEndpoint() : base(Component.Roles) { }
}