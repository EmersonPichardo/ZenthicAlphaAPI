using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Text.Json;

namespace Infrastructure.Behaviors;

internal class LoggingBehavior<TRequest>(
    ILogger<TRequest>? logger
)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(
        TRequest request,
        CancellationToken cancellationToken)
    {
        logger?.LogInformation("Processing request: {@RequestName} {@RequestBody}",
            typeof(TRequest).Name,
            JsonSerializer.Serialize(request)
        );

        await Task.CompletedTask;
    }
}

internal class LoggingBehavior<TRequest, TResponse>(
    ILogger<TRequest>? logger
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
        var responseBody = JsonSerializer.Serialize(response.Value);

        logger?.LogInformation("Request processed: {@RequestName} {@RequestBody} {@ResponseBody}",
            typeof(TRequest).Name,
            JsonSerializer.Serialize(request),
            responseBody
        );

        await Task.CompletedTask;
    }
}