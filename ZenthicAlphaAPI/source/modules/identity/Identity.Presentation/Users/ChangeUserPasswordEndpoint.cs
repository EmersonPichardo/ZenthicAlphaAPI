﻿using Domain.Modularity;
using Identity.Application.Users.ChangePassword;
using MediatR;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;

namespace Identity.Presentation.Users;

public record ChangeUserPasswordEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Put;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/me/password"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender sender, ChangeUserPasswordCommand command) =>
    {
        var result = await sender.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
