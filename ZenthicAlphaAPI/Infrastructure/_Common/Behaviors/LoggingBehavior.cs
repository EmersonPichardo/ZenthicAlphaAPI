using Application._Common.Security.Authentication;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Text.Json;

namespace Infrastructure._Common.Behaviors;

internal class LoggingBehavior<TRequest>(
    ILogger<TRequest>? logger,
    IIdentityService identityService
)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(
        TRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserIdentityResult = identityService
            .GetCurrentUserIdentity();

        var currentUserId = currentUserIdentityResult.Match(
            currentUserIdentity => currentUserIdentity.Id.ToString(),
            none => "[Anonymous]",
            failure => "[Invalid user]"
        );

        logger?.LogInformation("Processing request: {@RequestName} {@RequestBody} {@RequestedBy}",
            typeof(TRequest).Name,
            JsonSerializer.Serialize(request),
            currentUserId
        );

        await Task.CompletedTask;
    }
}

internal class LoggingBehavior<TRequest, TResponse>(
    ILogger<TRequest>? logger,
    IIdentityService identityService
)
    : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IOneOf
{
    public async Task Process(
        TRequest request,
        TResponse response,
        CancellationToken cancellationToken)
    {
        var currentUserIdentityResult = identityService
            .GetCurrentUserIdentity();

        var currentUserId = currentUserIdentityResult.Match(
            currentUserIdentity => currentUserIdentity.Id.ToString(),
            none => "[Anonymous]",
            failure => "[Invalid user]"
        );

        var responseBody = JsonSerializer.Serialize(response.Value);

        logger?.LogInformation("Request processed: {@RequestName} {@RequestBody} {@ResponseBody} {@RequestedBy}",
            typeof(TRequest).Name,
            JsonSerializer.Serialize(request),
            responseBody,
            currentUserId
        );

        await Task.CompletedTask;
    }
}