using Identity.Application._Common.Settings;

namespace Identity.Application._Common.Authentication;

public interface IJwtService
{
    JwtSettings GetSettings();
    string GenerateJwtToken(Guid id);
    string GenerateJwtRefreshToken(Guid id);
}
