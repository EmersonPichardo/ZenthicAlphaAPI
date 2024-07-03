
namespace Application._Common.Failures;

public record GenericFailure : Failure
{
    private GenericFailure() { }

    internal static GenericFailure New(
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
