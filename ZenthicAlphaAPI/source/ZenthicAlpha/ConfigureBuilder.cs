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

        var endpointCollections = endpoints
            .GroupBy(
                endpoint => endpoint.Component,
                (component, endpoints) => new
                {
                    Component = component,
                    Endpoints = endpoints.ToArray()
                }
            )
            .ToArray();

        var registrationActions = endpointCollections
            .Select(collection => new Action(() =>
                {
                    var isRoot = collection.Component is 0;

                    var group = app.MapGroup(
                        isRoot ? "api" : $"api/{collection.Component.ToString().ToLower()}"
                    );

                    foreach (var endpoint in collection.Endpoints)
                    {
                        foreach (var route in endpoint.Routes)
                        {
                            var endpointBuilder = endpoint.Verbose switch
                            {
                                HttpVerbose.Get => group.MapGet(route, endpoint.Handler),
                                HttpVerbose.Post => group.MapPost(route, endpoint.Handler),
                                HttpVerbose.Put => group.MapPut(route, endpoint.Handler),
                                HttpVerbose.Patch => group.MapPatch(route, endpoint.Handler),
                                HttpVerbose.Delete => group.MapDelete(route, endpoint.Handler),
                                _ => throw new NotImplementedException($"Verbose {endpoint.Verbose} is unsupported")
                            };

                            endpointBuilder
                                .AllowAnonymous()
                                .WithTags($"{(isRoot ? ".Root" : collection.Component)}Endpoints")
                                .Produces(401, typeof(ProblemDetails))
                                .Produces(403, typeof(ProblemDetails))
                                .Produces(500, typeof(ProblemDetails));

                            foreach (var successType in endpoint.SuccessTypes)
                                endpointBuilder.Produces((int)endpoint.SuccessStatusCode, successType);
                        }
                    }
                }
            ))
            .ToArray();

        Parallel.Invoke(registrationActions);

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
