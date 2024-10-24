using Application.Failures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Result;

public static class ProblemFactory
{
    public static ProblemDetails CreateFromFailure(Failure failure) => failure switch
    {
        GenericFailure genericFailure
            => Generic(genericFailure),

        UnauthorizedAccessFailure unauthorizedAccessFailure
            => UnauthorizedAccess(unauthorizedAccessFailure),

        ForbiddenAccessFailure forbiddenAccessFailure
            => ForbiddenAccess(forbiddenAccessFailure),

        NotFoundFailure notFoundFailure
            => NotFound(notFoundFailure),

        InvalidRequestFailure invalidRequestFailure
            => InvalidRequest(invalidRequestFailure),

        _ => InternalServer()
    };

    private static ProblemDetails Generic(GenericFailure failure) => new()
    {
        Status = StatusCodes.Status500InternalServerError,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        Extensions = { { "extensions", failure.Extensions } }
    };
    private static ProblemDetails UnauthorizedAccess(UnauthorizedAccessFailure failure) => new()
    {
        Status = StatusCodes.Status401Unauthorized,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
        Extensions = { { "extensions", failure.Extensions } }
    };
    private static ProblemDetails ForbiddenAccess(ForbiddenAccessFailure failure) => new()
    {
        Status = StatusCodes.Status403Forbidden,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
        Extensions = { { "extensions", failure.Extensions } }
    };
    private static ProblemDetails NotFound(NotFoundFailure failure) => new()
    {
        Status = StatusCodes.Status404NotFound,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
        Extensions = { { "extensions", failure.Extensions } }
    };
    private static ProblemDetails InvalidRequest(InvalidRequestFailure failure) => new()
    {
        Status = StatusCodes.Status422UnprocessableEntity,
        Title = failure.Title,
        Detail = failure.Detail,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        Extensions = { { "extensions", failure.Extensions } }
    };
    private static ProblemDetails InternalServer() => new()
    {
        Status = StatusCodes.Status500InternalServerError,
        Title = "Un error inesperado ha ocurrido",
        Detail = "Por favor intente en otro momento",
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
    };
}
