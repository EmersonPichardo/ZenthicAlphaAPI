namespace Identity.Application.Auth;

public record JwtTokenDto
{
    public required DateTime ExpirationDate { get; init; }
    public required string Value { get; init; }
}