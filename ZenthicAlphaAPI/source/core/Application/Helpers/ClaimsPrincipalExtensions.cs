using Application.Failures;
using OneOf;
using System.Security.Claims;
using System.Text.Json;

namespace Application.Helpers;

public static class ClaimsPrincipalExtensions
{
    private static readonly string invalidSessionTitle = "Sesión inválida";

    public static bool HasClaim(this ClaimsPrincipal principal, string name)
    {
        return principal.HasClaim(claim => claim.Type.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static OneOf<string, Failure> GetStringByName(this ClaimsPrincipal value, string name)
    {
        var claim = value.FindFirst(name);

        return claim is not null
            ? claim.Value
            : FailureFactory.NotFound(invalidSessionTitle, $"No se encontró el valor {name} en la sesión");
    }

    public static OneOf<Guid, Failure> GetGuidByName(this ClaimsPrincipal value, string name)
    {
        var stringResult = value.GetStringByName(name);

        return stringResult.Match<OneOf<Guid, Failure>>(
            stringValue => Guid.TryParse(stringValue, out var guidValue)
                ? guidValue
                : FailureFactory.NotFound(invalidSessionTitle, $"El formato del valor {name} es inválido")
            ,
            failure => failure
        );
    }

    public static OneOf<TEnum, Failure> GetEnumByName<TEnum>(this ClaimsPrincipal value, string name)
        where TEnum : struct, Enum
    {
        var stringResult = value.GetStringByName(name);

        return stringResult.Match<OneOf<TEnum, Failure>>(
            stringValue => Enum.TryParse<TEnum>(stringValue, out var enumValue)
                ? enumValue
                : FailureFactory.NotFound(invalidSessionTitle, $"El formato del valor {name} es inválido")
            ,
            failure => failure
        );
    }

    public static OneOf<TResult, Failure> GetJsonByName<TResult>(this ClaimsPrincipal value, string name)
        where TResult : class
    {
        var stringResult = value.GetStringByName(name);

        return stringResult.Match<OneOf<TResult, Failure>>(
            stringValue =>
            {
                try { return JsonSerializer.Deserialize<TResult>(stringValue)!; }
                catch { return FailureFactory.NotFound(invalidSessionTitle, $"El formato del valor {name} es inválido"); }
            },
            failure => failure
        );
    }
}