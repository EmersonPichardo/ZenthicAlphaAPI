using Application.Commands;

namespace Identity.Application.OAuth.Add;

public record AddOAuthCommand
    : ICommand<Guid>
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
}
