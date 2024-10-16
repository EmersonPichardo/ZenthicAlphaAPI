using Application.Auth;
using Domain.Modularity;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace Identity.Infrastructure.Common.Auth;

internal class JwtManager(
    IOptions<AuthSettings> authSettingsOptions,
    IHostEnvironment environment
)
{
    public const string RefreshTokenIdentifier = "IsRefreshToken";

    private readonly AuthSettings.JwtSettings jwtSettings = authSettingsOptions.Value.Jwt;

    public string GenerateJwtToken(User user, IReadOnlyDictionary<string, Permission> accesses)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)
        );

        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: environment.ApplicationName,
            expires: DateTime.UtcNow.Add(jwtSettings.TokenLifetime),
            signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512),
            claims: [
                new(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new(nameof(AuthenticatedSession.Id), user.Id.ToString()),
                new(nameof(AuthenticatedSession.UserName), user.UserName),
                new(nameof(AuthenticatedSession.Email), user.Email),
                new(nameof(AuthorizedSession.Status), user.Status.ToString()),
                new(nameof(AuthorizedSession.Accesses), JsonSerializer.Serialize(accesses))
            ]
        );

        var jwtToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return jwtToken;
    }

    public string GenerateJwtRefreshToken(Guid userId)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)
        );

        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: environment.ApplicationName,
            expires: DateTime.UtcNow.Add(jwtSettings.RefreshTokenLifetime),
            signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512),
            claims: [
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(nameof(RefreshTokenSession.UserId), userId.ToString()),
                new(RefreshTokenIdentifier, true.ToString())
            ]
        );

        var jwtToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return jwtToken;
    }
}
