using Application.Helpers;
using System.Security.Claims;

namespace Identity.Application._Common.Helpers;

public static class ClaimExtensions
{
    public static string? GetByName(this IEnumerable<Claim> value, string name)
    {
        return value
            .FirstOrDefault(
                claim => claim.Type.ToNormalize().Equals(name.ToNormalize())
            )?
            .Value;
    }
}