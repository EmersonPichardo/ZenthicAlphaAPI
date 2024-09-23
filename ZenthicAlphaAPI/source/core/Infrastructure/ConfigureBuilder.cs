using Application;
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
using System.Reflection;

namespace Infrastructure;

public static class ConfigureBuilder
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddNotificationsServices()
            .AddAutoMapperServices()
            .AddFluentValidationServices()
            .AddCacheServices(configuration)
            .AddHealthChecksServices(configuration)
            .AddMediatorServices();

        return builder;
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
    private static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
    {
        //services.AddAutoMapper(
        //    typeof(IApplicationAssembly).Assembly,
        //    Assembly.GetExecutingAssembly()
        //    );

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