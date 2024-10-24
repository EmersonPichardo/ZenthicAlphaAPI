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

    public string Generate(JwtRequest jwtRequest)
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
                new(nameof(AuthenticatedSession.Id), jwtRequest.Id),
                new(nameof(AuthenticatedSession.UserName), jwtRequest.UserName),
                new(nameof(AuthenticatedSession.Email), jwtRequest.Email),
                new(nameof(AuthorizedSession.Status), jwtRequest.Status),
                new(nameof(AuthorizedSession.Accesses), JsonSerializer.Serialize(jwtRequest.Accesses))
            ]
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);
    }
    public string GenerateRefreshToken()
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
                new(RefreshTokenIdentifier, true.ToString())
            ]
        );

        var jwtToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return jwtToken;
    }

    public record JwtRequest
    {
        public required string Id { get; init; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Status { get; set; }
        public IReadOnlyDictionary<string, Permission> Accesses { get; set; } = new Dictionary<string, Permission>();

        public static JwtRequest FromUser(User user, IReadOnlyDictionary<string, Permission> accesses) => new()
        {
            Id = user.Id.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            Status = user.Status.ToString(),
            Accesses = accesses
        };
    }
}
