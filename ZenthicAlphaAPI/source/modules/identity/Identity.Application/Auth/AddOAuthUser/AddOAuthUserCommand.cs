using Application.Auth;
using Application.Commands;

namespace Identity.Application.Auth.AddOAuthUser;

[AuthorizeOAuth]
public record AddOAuthUserCommand
    : ICommand<AddOAuthUserCommandResponse>;
