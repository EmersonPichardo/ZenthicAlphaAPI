using Application.Caching;
using Application.Events;
using Application.Exceptions;
using Application.Notifications.Emails;
using Application.Settings;
using FluentValidation;
using Infrastructure.Behaviors;
using Infrastructure.Caching;
using Infrastructure.Events;
using Infrastructure.Notification.Email;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using System.Reflection;

namespace Infrastructure;

public static class ConfigureBuilder
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddLoggerServices(builder)
            .AddNotificationsServices()
            .AddFluentValidationServices()
            .AddCacheServices(configuration)
            .AddHealthChecksServices(configuration)
            .AddMediatorServices();

        return builder;
    }
    private static IServiceCollection AddLoggerServices(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        services.AddLogging(loggingOptions => loggingOptions.AddOpenTelemetry(openTelemetryOptions => {
            openTelemetryOptions.SetResourceBuilder(
                ResourceBuilder.CreateEmpty()
                .AddService(builder.Environment.ApplicationName)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = builder.Environment.EnvironmentName
                })
            );

            openTelemetryOptions.IncludeFormattedMessage = true;
            openTelemetryOptions.IncludeScopes = true;

            openTelemetryOptions.AddOtlpExporter(exporterOptions =>
            {
                exporterOptions.Endpoint = new("http://zenthicAlpha.logger/ingest/otlp/v1/logs");
                exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                exporterOptions.Headers = "X-Seq-ApiKey=qXCj2RQpmTpFH3o6MdRd";
            });
        }));

        return services;
    }
    private static IServiceCollection AddNotificationsServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IEventPublisher, EventPublisher>()
            .AddTransient<IEmailSender, EmailSender>()
            .AddHostedService<EventProcessor>();

        services.Configure<HostOptions>(options =>
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore
        );

        return services;
    }
    private static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly(),
            includeInternalTypes: true
        );

        return services;
    }
    private static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheSettings = configuration
            .GetRequiredSection(nameof(CacheSettings))
            .Get<CacheSettings>()
        ?? throw new NotFoundException($"Setting {nameof(CacheSettings)} was not found.");

        services
            .AddStackExchangeRedisCache(options =>
                options.Configuration = cacheSettings.ConnectionString
            )
            .AddSingleton<ICacheStore, CacheStore>();

        return services;
    }
    private static IServiceCollection AddHealthChecksServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheSettings = configuration
            .GetRequiredSection(nameof(CacheSettings))
            .Get<CacheSettings>()
        ?? throw new NotFoundException($"Setting {nameof(CacheSettings)} was not found.");

        services
            .AddHealthChecks()
            .AddRedis(cacheSettings.ConnectionString)
            .AddCheck<EmailHealthCheck>("Email");

        return services;
    }
    private static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            configuration.AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
            configuration.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenRequestPostProcessor(typeof(LoggingBehavior<,>));

            configuration.NotificationPublisher = new TaskWhenAllPublisher();
            configuration.NotificationPublisherType = typeof(TaskWhenAllPublisher);
            configuration.Lifetime = ServiceLifetime.Transient;
        });

        return services;
    }
}