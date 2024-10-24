using Application.Failures;
using Microsoft.AspNetCore.Http;

namespace Presentation.Result;

public static class ResultFactory
{
    public static IResult Ok<TResponse>(TResponse? response)
        => Results.Ok(response);

    public static IResult Created<TResponse>(TResponse? response)
        => Results.Created(string.Empty, response);

    public static IResult ProblemDetails(Failure failure)
        => Results.Problem(ProblemFactory.CreateFromFailure(failure));
}
