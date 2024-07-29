using Application._Common.Failures;
using Microsoft.AspNetCore.Mvc;

namespace Presentation._Common.ExceptionHandler;

internal static class ProblemFactory
{
    internal static ProblemDetails Generic(GenericFailure failure) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        Extensions = { { "extensions", failure.Extensions } }
    };

    internal static ProblemDetails UnauthorizedAccess(UnauthorizedAccessFailure failure) => new()
    {
        Status = StatusCodes.Status401Unauthorized,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
        Extensions = { { "extensions", failure.Extensions } }
    };

    internal static ProblemDetails ForbiddenAccess(ForbiddenAccessFailure failure) => new()
    {
        Status = StatusCodes.Status403Forbidden,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
        Extensions = { { "extensions", failure.Extensions } }
    };

    internal static ProblemDetails NotFound(NotFoundFailure failure) => new()
    {
        Status = StatusCodes.Status404NotFound,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
        Extensions = { { "extensions", failure.Extensions } }
    };

    internal static ProblemDetails InvalidRequest(InvalidRequestFailure failure) => new()
    {
        Status = StatusCodes.Status422UnprocessableEntity,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        Extensions = { { "extensions", failure.Extensions } }
    };

    internal static ProblemDetails InternalServer() => new()
    {
        Status = StatusCodes.Status500InternalServerError,
        Title = "Un error inesperado ha ocurrido",
        Detail = "Por favor intente en otro momento",
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
    };
}
