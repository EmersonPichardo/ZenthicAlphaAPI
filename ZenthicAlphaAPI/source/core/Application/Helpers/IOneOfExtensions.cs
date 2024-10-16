using Application.Failures;
using OneOf;
using OneOf.Types;

namespace Application.Helpers;

public static class IOneOfExtensions
{
    public static bool IsSuccess(this IOneOf result)
        => result.Value is Success or not Failure;

    public static bool IsFailure(this IOneOf result)
        => result.Value is Error or Failure;

    public static bool IsNull(this IOneOf result)
        => result.Value is None;

    public static T GetValue<T>(this IOneOf result)
        => (T)result.Value;

    public static Failure GetFailure(this IOneOf result)
        => result.IsFailure()
            ? result.GetValue<Failure>()
            : throw new InvalidOperationException($"Cannot return value as Failure as result is {result.GetType().Name}");
}
