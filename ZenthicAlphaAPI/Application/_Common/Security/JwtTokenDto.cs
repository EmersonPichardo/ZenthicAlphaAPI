namespace Application._Common.Security;

public record JwtTokenDto
{
    public required DateTime ExpirationDate { get; init; }
    public required string Value { get; init; }
}