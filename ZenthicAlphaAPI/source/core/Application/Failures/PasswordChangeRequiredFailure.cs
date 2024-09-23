namespace Application.Failures;

public record PasswordChangeRequiredFailure : Failure
{
    public PasswordChangeRequiredFailure() { }

    internal static PasswordChangeRequiredFailure New(
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
