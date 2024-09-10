namespace Application.Failures;

public record ForbiddenAccessFailure : Failure
{
    private ForbiddenAccessFailure() { }

    internal static ForbiddenAccessFailure New(
        string? detail = null,
        IDictionary<string, object?>? extensions = null
    ) => new()
    {
        Title = "Permisos insuficientes",
        Detail = detail,
        Extensions = extensions ?? new Dictionary<string, object?>()
    };

    internal static ForbiddenAccessFailure New(
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
