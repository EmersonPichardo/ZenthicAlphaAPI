using Application._Common.Security;
using Domain.Security;

namespace Application.Users.Login;

public record LoginUserCommandResponse
{
    public required string DisplayName { get; init; }
    public required UserStatus Status { get; init; }
    public required JwtTokenDto RefreshToken { get; init; }
    public required JwtTokenDto Token { get; init; }
    public required IDictionary<string, int> Access { get; init; }
}