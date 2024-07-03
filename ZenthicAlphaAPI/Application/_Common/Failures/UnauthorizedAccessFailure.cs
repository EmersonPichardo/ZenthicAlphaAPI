
namespace Application._Common.Failures;

public record UnauthorizedAccessFailure : Failure
{
    private UnauthorizedAccessFailure() { }

    internal static UnauthorizedAccessFailure New(
        string? detail = null,
        IDictionary<string, object?>? extensions = null
    ) => new()
    {
        Title = "Acceso denegado",
        Detail = detail,
        Extensions = extensions ?? new Dictionary<string, object?>()
    };

    internal static UnauthorizedAccessFailure New(
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
