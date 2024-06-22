namespace Application._Common.Security.Authentication;

public interface IJwtService
{
    JwtSettings GetSettings();
    string GenerateJwtToken(Guid id);
    string GenerateJwtRefreshToken(Guid id);
}
