using Application._Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation._Common.Middleware.ExceptionHandler;

internal class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem = exception switch
        {
            UnauthorizedAccessException
                => ProblemDetailsFactory.UnauthorizedAccessProblem(httpContext, exception),

            ForbiddenAccessException or PasswordChangeRequiredException
                => ProblemDetailsFactory.ForbiddenAccessProblem(httpContext, exception),

            NotFoundException
                => ProblemDetailsFactory.NotFoundProblem(httpContext, exception),

            ValidationException validationException
                => ProblemDetailsFactory.ValidationProblem(httpContext, validationException),

            _ => ProblemDetailsFactory.InternalServerProblem(httpContext, exception)
        };

        httpContext.Response.StatusCode = problem.Status.GetValueOrDefault();
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
