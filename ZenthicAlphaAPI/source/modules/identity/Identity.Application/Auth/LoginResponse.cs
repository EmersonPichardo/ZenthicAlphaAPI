namespace Identity.Application.Auth;

public record LoginResponse
{
    public required string UserName { get; init; }
    public required IReadOnlyCollection<string> Statuses { get; init; }
    public required IDictionary<string, string[]> Accesses { get; init; }
    public required JwtTokenDto AccessToken { get; init; }
    public required JwtTokenDto RefreshToken { get; init; }
}
