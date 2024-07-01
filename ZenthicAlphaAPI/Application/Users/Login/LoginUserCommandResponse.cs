using Application._Common.Security;

namespace Application.Users.Login;

public record LoginUserCommandResponse
{
    public required string DisplayName { get; init; }
    public required string Status { get; init; }
    public required JwtTokenDto RefreshToken { get; init; }
    public required JwtTokenDto Token { get; init; }
    public required IDictionary<string, int> Access { get; init; }
}