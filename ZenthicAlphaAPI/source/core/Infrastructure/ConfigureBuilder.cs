using Application.Behaviors;
using Application.Caching;
using Application.Events;
using Application.Exceptions;
using Application.Notifications.Emails;
using FluentValidation;
using Infrastructure.Behaviors;
using Infrastructure.Caching;
using Infrastructure.Events;
using Infrastructure.Notification.Email;
using Infrastructure.Persistence.Databases;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Reflection;

namespace Infrastructure;

public static partial class ConfigureBuilder
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder
            .AddDbContextServices()
            .AddOpenTelemetryServices()
            .AddNotificationsServices()
            .AddFluentValidationServices()
            .AddCacheServices()
            .AddHealthChecksServices()
            .AddMediatorServices();

        return builder;
    }

    private static WebApplicationBuilder AddDbContextServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return builder;
    }
    private static WebApplicationBuilder AddOpenTelemetryServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<OpenTelemetrySettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(OpenTelemetrySettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Logging.ClearProviders();

        var applicationName = builder.Environment.ApplicationName;
        var activitySourceName = $"{applicationName}.activitySource";

        var openTelemetrySettings = builder.Configuration
            .GetRequiredSection(nameof(OpenTelemetrySettings))
            .Get<OpenTelemetrySettings>()
        ?? throw new NotFoundException($"Setting {nameof(OpenTelemetrySettings)} was not found.");

        var baseEndpoint = new Uri(openTelemetrySettings.IngestBaseEndpoint);
        var protocol = Enum.Parse<OtlpExportProtocol>(openTelemetrySettings.OtlpExportProtocol);
        var securityHeader = $"X-Seq-ApiKey={openTelemetrySettings.ApiKey}";

        builder.Services
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
                    exporterOptions.Endpoint = new(baseEndpoint, openTelemetrySettings.LoggingPartialEndpoint);
                    exporterOptions.Protocol = protocol;
                    exporterOptions.Headers = securityHeader;
                });
            })
            .WithTracing(tracingOptions =>
            {
                tracingOptions.AddSource(activitySourceName);

                tracingOptions.AddAspNetCoreInstrumentation();
                tracingOptions.AddHttpClientInstrumentation();
                tracingOptions.AddSqlClientInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.EnableConnectionLevelAttributes = true;
                });

                tracingOptions.AddOtlpExporter(exporterOptions =>
                {
                    exporterOptions.Endpoint = new(baseEndpoint, openTelemetrySettings.TracingPartialEndpoint);
                    exporterOptions.Protocol = protocol;
                    exporterOptions.Headers = securityHeader;
                });
            });

        return builder;
    }
    private static WebApplicationBuilder AddNotificationsServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<SmtpSettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(SmtpSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddSingleton<IEventPublisher, EventPublisher>()
            .AddTransient<IEmailSender, EmailSender>()
            .AddHostedService<EventProcessor>();

        builder.Services
            .AddOptions<BackgroundTaskSettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(BackgroundTaskSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.Configure<HostOptions>(options =>
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore
        );

        return builder;
    }
    private static WebApplicationBuilder AddFluentValidationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly(),
            includeInternalTypes: true
        );

        return builder;
    }
    private static WebApplicationBuilder AddCacheServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<CacheSettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(CacheSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var cacheSettings = builder.Configuration
            .GetRequiredSection(nameof(CacheSettings))
            .Get<CacheSettings>()
        ?? throw new NotFoundException($"Setting {nameof(CacheSettings)} was not found.");

        IConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.Connect(
            cacheSettings.ConnectionString,
            options => options.AbortOnConnectFail = builder.Environment.IsEnvironment("Local")
        );

        builder.Services
            .AddSingleton(redisConnectionMultiplexer)
            .AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnectionMultiplexer)
            )
            .AddSingleton<ICacheStore, CacheStore>();

        return builder;
    }
    private static WebApplicationBuilder AddHealthChecksServices(this WebApplicationBuilder builder)
    {
        var cacheSettings = builder.Configuration
            .GetRequiredSection(nameof(CacheSettings))
            .Get<CacheSettings>()
        ?? throw new NotFoundException($"Setting {nameof(CacheSettings)} was not found.");

        builder.Services
            .AddHealthChecks()
            .AddRedis(cacheSettings.ConnectionString)
            .AddCheck<EmailHealthCheck>("Email");

        return builder;
    }
    private static WebApplicationBuilder AddMediatorServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<PerformanceSettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(PerformanceSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            configuration.AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
            configuration.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            configuration.AddOpenRequestPostProcessor(typeof(LoggingBehavior<,>));

            configuration.NotificationPublisher = new TaskWhenAllPublisher();
            configuration.NotificationPublisherType = typeof(TaskWhenAllPublisher);
            configuration.Lifetime = ServiceLifetime.Transient;
        });

        return builder;
    }
}