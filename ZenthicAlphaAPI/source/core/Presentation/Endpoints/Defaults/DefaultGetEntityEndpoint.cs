using Application.Queries;
using Domain.Modularity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Presentation.Result;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultGetEntityEndpoint<TQuery, TResponse>(Component Component) : IEndpoint
    where TQuery : GetEntityQuery<TResponse>, new()
{
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/{id:guid}"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [typeof(TResponse)];
    public Delegate Handler { get; init; } = async (
        ISender mediator, Guid id, CancellationToken cancellationToken) =>
    {
        return await GetEntityResultAsync(mediator, id, cancellationToken);
    };

    public static async Task<IResult> GetEntityResultAsync(ISender mediator, Guid id, CancellationToken cancellationToken)
    {
        var query = new TQuery { Id = id };
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
}
