using FluentValidation.Results;

namespace Application.Failures;

public static class FailureFactory
{
    public static GenericFailure Generic(string title, string? detail = null, IDictionary<string, object?>? extensions = null)
        => GenericFailure.New(title, detail, extensions);

    public static NotFoundFailure NotFound(string title, string? detail = null, IDictionary<string, object?>? extensions = null)
        => NotFoundFailure.New(title, detail, extensions);

    public static InvalidRequestFailure InvalidRequest(string title, string? detail = null, IDictionary<string, object?>? extensions = null)
        => InvalidRequestFailure.New(title, detail, extensions);
    public static InvalidRequestFailure InvalidRequest(string title, IEnumerable<ValidationFailure> failures)
        => InvalidRequestFailure.New(title, failures);

    public static UnauthorizedAccessFailure UnauthorizedAccess(string title, string? detail = null, IDictionary<string, object?>? extensions = null)
        => UnauthorizedAccessFailure.New(title, detail, extensions);
    public static UnauthorizedAccessFailure UnauthorizedAccess(string? detail = null, IDictionary<string, object?>? extensions = null)
        => UnauthorizedAccessFailure.New(detail, extensions);

    public static ForbiddenAccessFailure ForbiddenAccess(string title, string? detail = null, IDictionary<string, object?>? extensions = null)
        => ForbiddenAccessFailure.New(title, detail, extensions);
    public static ForbiddenAccessFailure ForbiddenAccess(string? detail = null, IDictionary<string, object?>? extensions = null)
        => ForbiddenAccessFailure.New(detail, extensions);

    public static PasswordChangeRequiredFailure PasswordChangeRequired(string title, string? detail = null, IDictionary<string, object?>? extensions = null)
        => PasswordChangeRequiredFailure.New(title, detail, extensions);
}
