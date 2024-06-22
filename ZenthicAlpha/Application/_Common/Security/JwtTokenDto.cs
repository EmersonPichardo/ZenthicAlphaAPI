namespace Application._Common.Security;

public record JwtTokenDto
{
    public required DateTime Expires { get; init; }
    public required string Value { get; init; }
}