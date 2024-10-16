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

            var tasks = eventPublisher
                .GetPendingEvents()
                .Select(
                    @event =>
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                        return publisher.Publish(@event, stoppingToken);
                    }
                );

            await Task.WhenAll(tasks);

        }
    }
}
