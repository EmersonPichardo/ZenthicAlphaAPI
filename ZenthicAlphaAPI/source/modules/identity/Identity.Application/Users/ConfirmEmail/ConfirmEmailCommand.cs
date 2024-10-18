using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Users.ConfirmEmail;

[Cache(Component.Users)]
public record ConfirmEmailCommand
    : ICommand
{
    public required string Token { get; init; }
}
