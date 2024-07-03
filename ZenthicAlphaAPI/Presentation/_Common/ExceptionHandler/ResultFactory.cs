using Application._Common.Failures;

namespace Presentation._Common.ExceptionHandler;

internal static class ResultFactory
{
    internal static IResult Ok<TResponse>(TResponse? response)
        => Results.Ok(response);

    internal static IResult Created<TResponse>(TResponse? response)
        => Results.Created(string.Empty, response);

    internal static IResult ProblemDetails(Failure failure) => failure switch
    {
        GenericFailure genericFailure
            => Results.Problem(ProblemFactory.Generic(genericFailure)),

        UnauthorizedAccessFailure unauthorizedAccessFailure
            => Results.Problem(ProblemFactory.UnauthorizedAccess(unauthorizedAccessFailure)),

        ForbiddenAccessFailure forbiddenAccessFailure
            => Results.Problem(ProblemFactory.ForbiddenAccess(forbiddenAccessFailure)),

        NotFoundFailure notFoundFailure
            => Results.Problem(ProblemFactory.NotFound(notFoundFailure)),

        InvalidRequestFailure invalidRequestFailure
            => Results.Problem(ProblemFactory.InvalidRequest(invalidRequestFailure)),

        _ => Results.Problem(ProblemFactory.InternalServer())
    };
}
