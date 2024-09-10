using Application.Failures;

namespace Presentation.Endpoints;

public static class ResultFactory
{
    public static IResult Ok<TResponse>(TResponse? response)
        => Results.Ok(response);

    public static IResult Created<TResponse>(TResponse? response)
        => Results.Created(string.Empty, response);

    public static IResult ProblemDetails(Failure failure) => failure switch
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
