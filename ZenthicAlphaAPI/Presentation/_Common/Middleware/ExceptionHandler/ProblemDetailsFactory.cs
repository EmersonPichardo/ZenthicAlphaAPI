using Application._Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Presentation._Common.Middleware.ExceptionHandler;

internal static class ProblemDetailsFactory
{
    public static ProblemDetails UnauthorizedAccessProblem
        (HttpContext httpContext, Exception exception) => new()
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Access denied.",
            Detail = exception.Message,
            Instance = ConstructInstance(httpContext),
            Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"
        };

    public static ProblemDetails ForbiddenAccessProblem
        (HttpContext httpContext, Exception exception) => new()
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Access denied.",
            Detail = exception.Message,
            Instance = ConstructInstance(httpContext),
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3"
        };

    public static ProblemDetails NotFoundProblem
        (HttpContext httpContext, Exception exception) => new()
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found.",
            Detail = exception.Message,
            Instance = ConstructInstance(httpContext),
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4"
        };

    public static ProblemDetails ValidationProblem
        (HttpContext httpContext, ValidationException validationException) => new()
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Validation error.",
            Detail = validationException.Message,
            Instance = ConstructInstance(httpContext),
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Extensions = {
                ["errors"] = validationException.Errors
            }
        };

    public static ProblemDetails InternalServerProblem
        (HttpContext httpContext, Exception exception) => new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An internal unexpected error happened.",
            Detail = exception.Message,
            Instance = ConstructInstance(httpContext),
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };

    private static string ConstructInstance(HttpContext httpContext)
        => $"[{httpContext.Request.Method}] {httpContext.Request.Path}";
}
