using Application._Common.Events;
using Application._Common.Settings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure._Common.Events;

internal class EventProcessor(
    IEventPublisher eventPublisher,
    IPublisher publisher,
    IOptions<BackgroundTaskSettings> backgroundTaskSettingsOptions
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

            var tasks = eventPublisher
                .GetPendingEvents()
                .Select(
                    @event => publisher.Publish(@event, stoppingToken)
                );

            Task.WhenAll(tasks);
        }
    }
}
