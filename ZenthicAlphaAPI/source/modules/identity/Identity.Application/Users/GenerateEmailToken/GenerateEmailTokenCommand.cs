using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Users.GenerateEmailToken;

[Cache(Component.Users)]
public record GenerateEmailTokenCommand
    : ICommand<string>
{
    public required Guid? UserId { get; init; }
}
