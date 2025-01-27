﻿using Application.Commands;
using Domain.Modularity;
using MediatR;
using Presentation.Result;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultDeleteEndpoint<TCommand>(Component Component) : IEndpoint
    where TCommand : IDeleteCommand, new()
{
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Delete;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/{id:guid}"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender sender, Guid id, CancellationToken cancellationToken) =>
    {
        var command = new TCommand { Id = id };
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
