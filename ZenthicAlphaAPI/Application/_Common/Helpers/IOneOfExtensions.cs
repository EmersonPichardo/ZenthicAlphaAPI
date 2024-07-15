using Application._Common.Failures;
using OneOf;
using OneOf.Types;

namespace Application._Common.Helpers;

public static class IOneOfExtensions
{
    public static bool IsSuccess(this IOneOf result)
        => result.Value is not Failure;

    public static bool IsNull(this IOneOf result)
        => result.Value is None;

    public static bool IsFailure(this IOneOf result)
        => result.Value is Failure;

    public static T GetValueAs<T>(this IOneOf result)
        => (T)result.Value;

    public static Failure GetValueAsFailure(this IOneOf result)
        => result.IsFailure()
            ? result.GetValueAs<Failure>()
            : throw new InvalidOperationException($"Cannot return value as Failure as result is {result.GetType().Name}");
}
