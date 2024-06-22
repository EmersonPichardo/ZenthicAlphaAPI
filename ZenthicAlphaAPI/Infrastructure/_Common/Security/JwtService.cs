using Application._Common.Security.Authentication;
using Application._Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure._Common.Security;

internal class JwtService(
    IOptions<JwtSettings> jwtSettingsOptions
)
    : IJwtService
{
    private readonly JwtSettings jwtSettings = jwtSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(jwtSettingsOptions));

    public JwtSettings GetSettings()
        => jwtSettings;

    public string GenerateJwtToken(Guid id)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(jwtSettings.Secret)
        );

        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: jwtSettings.Issuer,
            expires: DateTime.UtcNow.Add(jwtSettings.TokenLifetime),
            signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512),
            claims: [
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(nameof(ICurrentUserIdentity.Id), id.ToString())
            ]
        );

        var jwtToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return jwtToken;
    }

    public string GenerateJwtRefreshToken(Guid id)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(jwtSettings.Secret)
        );

        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: jwtSettings.Issuer,
            expires: DateTime.UtcNow.Add(jwtSettings.RefreshTokenLifetime),
            signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512),
            claims: [
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(nameof(ICurrentUserIdentity.Id), id.ToString()),
                new("IsRefreshToken", $"{true}")
            ]
        );

        var jwtToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return jwtToken;
    }
}
