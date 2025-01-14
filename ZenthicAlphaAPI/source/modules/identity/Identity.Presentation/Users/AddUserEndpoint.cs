﻿using Domain.Modularity;
using Identity.Application.Users.Add;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Users;

public record AddUserEndpoint() : DefaultAddEndpoint<AddUserCommand, AddUserCommandResponse>(Component.Users);
