using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace App.ExceptionHandler;

internal class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Un error inesperado ha ocurrido",
            Detail = "Por favor intente en otro momento",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };

        httpContext.Response.StatusCode = problem.Status.GetValueOrDefault();
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
