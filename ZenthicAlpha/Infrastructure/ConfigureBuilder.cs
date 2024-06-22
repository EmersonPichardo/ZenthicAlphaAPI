using Application;
using Application._Common.Caching;
using Application._Common.Events;
using Application._Common.Exceptions;
using Application._Common.Notifications.Emails;
using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Application._Common.Settings;
using FluentValidation;
using Infrastructure._Common.Behaviors;
using Infrastructure._Common.Caching;
using Infrastructure._Common.Events;
using Infrastructure._Common.Notification.Email;
using Infrastructure._Common.Security;
using Infrastructure._Persistence.Databases.ApplicationDbContext;
using Infrastructure._Persistence.Databases.Interceptors;
using MediatR.NotificationPublishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Infrastructure;

public static partial class ConfigureBuilder
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddNotificationsServices()
            .AddDbContextServices(configuration)
            .AddSecurityServices()
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
    private static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(nameof(ApplicationDbContext)),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                )
            );

        services
            .AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
    private static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services
            .AddTransient<IPasswordHasher, PasswordHasher>()
            .AddTransient<IJwtService, JwtService>();

        return services;
    }
    private static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(IApplicationAssembly).Assembly,
            Assembly.GetExecutingAssembly()
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
            .AddDbContextCheck<ApplicationDbContext>()
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
            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
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

public static partial class ConfigureBuilder
{
    public static IHost UseInfrastructure(this IHost host)
    {
        host.ApplyMigrations();

        return host;
    }

    private static IHost ApplyMigrations(this IHost host)
    {
        var environment = host.Services
            .GetRequiredService<IHostEnvironment>();

        if (!environment.IsDevelopment())
            return host;

        using var scope = host.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IApplicationDbContext>()
            .Database
            .MigrateAsync();

        return host;
    }
}