using Application.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Events;

internal class EventProcessor(
    IEventPublisher eventPublisher,
    IOptions<BackgroundTaskSettings> backgroundTaskSettingsOptions,
    IServiceScopeFactory serviceScopeFactory
)
    : BackgroundService
{
    private readonly PeriodicTimer timer = new(backgroundTaskSettingsOptions.Value.TimeInterval);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (
            !stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken)
        )
        {
            if (eventPublisher.HasNoPendingEvents())
                continue;

            using var asyncScope = serviceScopeFactory.CreateAsyncScope();
            var publisher = asyncScope.ServiceProvider.GetRequiredService<IPublisher>();

            var tasks = eventPublisher
                .GetPendingEvents()
                .Select(@event => publisher.Publish(@event, stoppingToken));

            await Task.WhenAll(tasks);
        }
    }
}
