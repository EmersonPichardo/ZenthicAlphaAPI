using Application.Helpers;
using HealthChecks.UI.Client;
using Infrastructure.Modularity;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Presentation.Endpoints;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZenthicAlpha.ExceptionHandler;

namespace ZenthicAlpha;

internal static partial class ConfigureBuilder
{
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder
            .AddHttpServices()
            .AddCorsServices()
            .AddOpenApiServices();

        return builder;
    }
    public static WebApplication UsePresentation(this WebApplication app)
    {
        if (app.Environment.IsProduction())
            app.UseHsts();

        app
            .UseApplicationCors()
            .UseAuth()
            .UseExceptionMiddleware()
            .UseSwaggerUserInterface()
            .UseHealthChecks()
            .UseEndpoints();

        return app;
    }

    private static WebApplicationBuilder AddHttpServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddHttpContextAccessor()
            .AddProblemDetails();

        return builder;
    }
    private static WebApplicationBuilder AddCorsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options => options
            .AddDefaultPolicy(configure => configure
                .AllowAnyOrigin()
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
            )
        );

        return builder;
    }
    private static WebApplicationBuilder AddOpenApiServices(this WebApplicationBuilder builder)
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

        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(ConfigureSwaggerGen)
            .AddFluentValidationRulesToSwagger();

        return builder;
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
    private static WebApplication UseAuth(this WebApplication app)
    {
        app
            .UseHttpsRedirection()
            .UseAuthentication();

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
            var group = app.MapGroup($"api/{collection.Component.ToString().ToNormalize()}");

            foreach (var endpoint in collection.Endpoints)
            {
                var endpointBuilders = endpoint.Verbose switch
                {
                    HttpVerbose.Get => endpoint.Routes.Select(route => group.MapGet(route, endpoint.Handler)),
                    HttpVerbose.Post => endpoint.Routes.Select(route => group.MapPost(route, endpoint.Handler)),
                    HttpVerbose.Put => endpoint.Routes.Select(route => group.MapPut(route, endpoint.Handler)),
                    HttpVerbose.Patch => endpoint.Routes.Select(route => group.MapPatch(route, endpoint.Handler)),
                    HttpVerbose.Delete => endpoint.Routes.Select(route => group.MapDelete(route, endpoint.Handler)),
                    _ => throw new NotImplementedException()
                };

                foreach (var endpointBuilder in endpointBuilders)
                {
                    endpointBuilder
                        .AllowAnonymous()
                        .WithTags($"{collection.Component}Endpoints")
                        .Produces(401, typeof(ProblemDetails))
                        .Produces(403, typeof(ProblemDetails))
                        .Produces(500, typeof(ProblemDetails));

                    foreach (var successType in endpoint.SuccessTypes)
                        endpointBuilder.Produces((int)endpoint.SuccessStatusCode, successType);
                }
            }
        }

        return app;
    }
}

internal static partial class ConfigureBuilder
{
    private static readonly IReadOnlyList<IModuleInstaller> installers = [
        new Identity.Infrastructure.Installer()
    ];

    public static WebApplicationBuilder AddModules(this WebApplicationBuilder builder)
    {
        foreach (var installer in installers)
            installer.AddInfrastructure(builder);

        return builder;
    }
    public static WebApplication UseModules(this WebApplication app)
    {
        foreach (var installer in installers)
            installer.UseInfrastructure(app);

        return app;
    }
}
