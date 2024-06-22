using Application._Common.Notifications.Emails;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure._Common.Notification.Email;

internal class EmailHealthCheck(
    IEmailSender emailSender
)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await emailSender.SendAsync(
                "jiyife1435@visignal.com",
                "Health check",
                string.Empty,
                cancellationToken
            );

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Degraded(exception: exception);
        }
    }
}
