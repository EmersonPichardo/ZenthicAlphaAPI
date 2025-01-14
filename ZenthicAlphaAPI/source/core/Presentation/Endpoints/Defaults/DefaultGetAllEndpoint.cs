﻿using Application.Queries;
using Domain.Modularity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Presentation.Result;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultGetAllEndpoint<TQuery, TResponse>(Component Component) : IEndpoint
    where TQuery : GetAllEntitiesQuery<TResponse>, new()
{
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [typeof(IEnumerable<TResponse>)];
    public Delegate Handler { get; init; } = async (
        ISender sender, CancellationToken cancellationToken) =>
    {
        return await GetAllQueryResultAsync(sender, cancellationToken);
    };

    public static async Task<IResult> GetAllQueryResultAsync(ISender sender, CancellationToken cancellationToken)
    {
        var query = new TQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
}
