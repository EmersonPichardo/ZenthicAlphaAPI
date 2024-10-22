using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ZenthicAlpha.ExceptionHandler;

internal class GlobalExceptionHandler(
    IHostEnvironment hostEnvironment
)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem = exception switch
        {
            BadHttpRequestException badHttpRequestException when badHttpRequestException is { InnerException: JsonException } => new()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = exception.Message,
                Detail = $"El formato del valor de {((JsonException)exception.InnerException!).Path} es incorrecto",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            },
            BadHttpRequestException badHttpRequestException when badHttpRequestException is { InnerException: null } => new()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = exception.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            },
            _ => new()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = hostEnvironment.IsDevelopment() ? exception.Message : "Un error inesperado ha ocurrido",
                Detail = hostEnvironment.IsDevelopment() ? exception.ToString() : "Por favor intente en otro momento",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            }
        };

        httpContext.Response.StatusCode = problem.Status.GetValueOrDefault();
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
