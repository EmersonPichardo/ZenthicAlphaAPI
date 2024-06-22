using Application._Common.Commands;
using Application._Common.Queries;
using Application._Common.Security.Authentication;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure._Common.Behaviors;

internal class LoggingBehavior<TRequest>(
    ILogger<TRequest>? logger,
    IIdentityService identityService
)
    : IRequestPreProcessor<TRequest>
    where TRequest : IBaseRequest
{
    public async Task Process(
        TRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = identityService
            .GetCurrentUserIdentity()?
            .Id
            .ToString()
        ?? "[Anonymous]";

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
    where TRequest : IBaseRequest
{
    public async Task Process(
        TRequest request,
        TResponse response,
        CancellationToken cancellationToken)
    {
        var currentUserId = identityService
            .GetCurrentUserIdentity()?
            .Id
            .ToString()
        ?? "[Anonymous]";

        var responseBody = request switch
        {
            IBaseQuery => "[Redacted]",
            IBaseCommand => JsonSerializer.Serialize(response),
            _ => throw new InvalidOperationException($"Unknown request type {{{request.GetType()}}}")
        };

        logger?.LogInformation("Request processed: {@RequestName} {@RequestBody} {@ResponseBody} {@RequestedBy}",
            typeof(TRequest).Name,
            JsonSerializer.Serialize(request),
            responseBody,
            currentUserId
        );

        await Task.CompletedTask;
    }
}