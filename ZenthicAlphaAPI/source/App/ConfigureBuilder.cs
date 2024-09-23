using App.ExceptionHandler;
using Application.Helpers;
using Application.Settings;
using HealthChecks.UI.Client;
using Infrastructure.Modularity;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Presentation.Endpoints;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App;

internal static partial class ConfigureBuilder
{
    public static IHostApplicationBuilder AddPresentation(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddCorsServices()
            .AddOpenApiServices()
            .AddSettingsServices(configuration);

        return builder;
    }
    public static WebApplication UsePresentation(this WebApplication app)
    {
        if (app.Environment.IsProduction())
            app.UseHsts();

        app
            .UseApplicationCors()
            .UseSecurity()
            .UseExceptionMiddleware()
            .UseSwaggerUserInterface()
            .UseHealthChecks()
            .UseEndpoints();

        return app;
    }

    private static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        services.AddCors(options => options
            .AddDefaultPolicy(configure => configure
                .AllowAnyOrigin()
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
            )
        );

        return services;
    }
    private static IServiceCollection AddOpenApiServices(this IServiceCollection services)
    {
        static void ConfigureSwaggerGen(SwaggerGenOptions setupAction)
        {
            var openApiSecurityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Description = "JWT authorization header using the bearer scheme",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Reference = new()
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };

            setupAction.AddSecurityDefinition("Bearer", openApiSecurityScheme);
            setupAction.AddSecurityRequirement(new() { { openApiSecurityScheme, [] } });

            setupAction.MapType(typeof(TimeSpan), () => new()
            {
                Type = "string",
                Example = new OpenApiString("00:00:00")
            });
        }

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(ConfigureSwaggerGen)
            .AddFluentValidationRulesToSwagger();

        return services;
    }
    private static IServiceCollection AddSettingsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<CacheSettings>()
            .Bind(configuration.GetRequiredSection(nameof(CacheSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services
            .AddOptions<BackgroundTaskSettings>()
            .Bind(configuration.GetRequiredSection(nameof(BackgroundTaskSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services
            .AddOptions<SmtpSettings>()
            .Bind(configuration.GetRequiredSection(nameof(SmtpSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services
            .AddOptions<PerformanceSettings>()
            .Bind(configuration.GetRequiredSection(nameof(PerformanceSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    private static WebApplication UseApplicationCors(this WebApplication app)
    {
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );

        return app;
    }
    private static WebApplication UseSecurity(this WebApplication app)
    {
        app
            .UseHttpsRedirection()
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }
    private static WebApplication UseExceptionMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler();

        return app;
    }
    private static WebApplication UseSwaggerUserInterface(this WebApplication app)
    {
        app
            .UseSwagger()
            .UseSwaggerUI();

        return app;
    }
    private static WebApplication UseHealthChecks(this WebApplication app)
    {
        app
            .MapHealthChecks("/health", new()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        return app;
    }
    private static WebApplication UseEndpoints(this WebApplication app)
    {
        var assembly = Identity.Presentation.AssemblyReference.Assembly;
        var endpoints = AbstractHelpers.CreateInstancesAssignableFromType<IEndpoint>(assembly);
        var endpointCollections = endpoints.GroupBy(
            endpoint => endpoint.Component,
            (key, collection) => new { Component = key, Endpoints = collection }
        );

        foreach (var collection in endpointCollections)
        {
            var group = app.MapGroup($"api/{collection.Component}");

            foreach (var endpoint in collection.Endpoints)
            {
                var endpointBuilder = endpoint.Verbose switch
                {
                    HttpVerbose.Get => group.MapGet(endpoint.Route, endpoint.Handler),
                    HttpVerbose.Post => group.MapPost(endpoint.Route, endpoint.Handler),
                    HttpVerbose.Put => group.MapPut(endpoint.Route, endpoint.Handler),
                    HttpVerbose.Patch => group.MapPatch(endpoint.Route, endpoint.Handler),
                    HttpVerbose.Delete => group.MapDelete(endpoint.Route, endpoint.Handler),
                    _ => throw new NotImplementedException()
                };
                endpointBuilder
                    .AllowAnonymous()
                    .WithTags($"{collection.Component}Endpoints")
                    .Produces((int)endpoint.SuccessStatusCode, endpoint.SuccessType)
                    .Produces(401, typeof(ProblemDetails))
                    .Produces(403, typeof(ProblemDetails))
                    .Produces(500, typeof(ProblemDetails));
            }
        }

        return app;
    }
}

internal static partial class ConfigureBuilder
{
    public static IHostApplicationBuilder AddModules(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddModulesInfrastructure(configuration);

        return builder;
    }
    public static IHost UseModules(this IHost host)
    {
        host.UseModulesInfrastructure();

        return host;
    }

    private static IServiceCollection AddModulesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var currentAssembly = Identity.Infrastructure.AssemblyReference.Assembly;
        var infrastructureInstallers = AbstractHelpers.CreateInstancesAssignableFromType<IInfrastructureInstaller>(currentAssembly);

        foreach (var installer in infrastructureInstallers)
            installer.AddInfrastructure(services, configuration);

        return services;
    }

    private static IHost UseModulesInfrastructure(this IHost host)
    {
        var currentAssembly = Identity.Infrastructure.AssemblyReference.Assembly;
        var infrastructureInstallers = AbstractHelpers.CreateInstancesAssignableFromType<IInfrastructureInstaller>(currentAssembly);

        foreach (var installer in infrastructureInstallers)
            installer.UseInfrastructure(host);

        return host;
    }
}
