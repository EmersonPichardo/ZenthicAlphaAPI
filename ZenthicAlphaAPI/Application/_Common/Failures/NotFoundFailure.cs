
namespace Application._Common.Failures;

public record NotFoundFailure : Failure
{
    private NotFoundFailure() { }

    internal static NotFoundFailure New(
        string title,
        string? detail = null,
        IDictionary<string, object?>? extensions = null
    ) => new()
    {
        Title = title,
        Detail = detail,
        Extensions = extensions ?? new Dictionary<string, object?>()
    };
}
