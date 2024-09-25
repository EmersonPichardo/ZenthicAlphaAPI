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
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Diagnostics;
using System.Reflection;

namespace Infrastructure;

public static class ConfigureBuilder
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddOpenTelemetryServices(builder, configuration)
            .AddNotificationsServices()
            .AddFluentValidationServices()
            .AddCacheServices(configuration)
            .AddHealthChecksServices(configuration)
            .AddMediatorServices();

        return builder;
    }
    private static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services, IHostApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Logging.ClearProviders();

        var applicationName = builder.Environment.ApplicationName;
        var environmentName = builder.Environment.EnvironmentName;

        var activitySourceName = $"{applicationName}.activitySource";
        var activitySource = new ActivitySource(activitySourceName);

        var baseEndpoint = new Uri("http://zenthicAlpha.logger/ingest/otlp/v1/");
        var protocol = OtlpExportProtocol.HttpProtobuf;
        var securityHeader = "X-Seq-ApiKey=qXCj2RQpmTpFH3o6MdRd";

        services
            .AddOpenTelemetry()
            .ConfigureResource(options => options
                .AddService(applicationName)
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector()
            )
            .WithLogging(null, loggingOptions =>
            {
                loggingOptions.IncludeFormattedMessage = true;
                loggingOptions.IncludeScopes = true;

                loggingOptions.AddOtlpExporter(exporterOptions =>
                {
                    exporterOptions.Endpoint = new(baseEndpoint, "./logs");
                    exporterOptions.Protocol = protocol;
                    exporterOptions.Headers = securityHeader;
                });
            })
            .WithTracing(tracingOptions =>
            {
                var cacheSettings = configuration
                    .GetRequiredSection(nameof(CacheSettings))
                    .Get<CacheSettings>()
                ?? throw new NotFoundException($"Setting {nameof(CacheSettings)} was not found.");

                tracingOptions.AddSource(activitySourceName);

                tracingOptions.AddAspNetCoreInstrumentation();
                tracingOptions.AddHttpClientInstrumentation();
                tracingOptions.AddSqlClientInstrumentation();

                tracingOptions.AddOtlpExporter(exporterOptions =>
                {
                    exporterOptions.Endpoint = new(baseEndpoint, "./traces");
                    exporterOptions.Protocol = protocol;
                    exporterOptions.Headers = securityHeader;
                });
            });

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

        IConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.Connect(cacheSettings.ConnectionString);

        services
            .AddSingleton(redisConnectionMultiplexer)
            .AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnectionMultiplexer)
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