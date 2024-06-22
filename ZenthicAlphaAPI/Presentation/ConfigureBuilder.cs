using Application._Common.Exceptions;
using Application._Common.Helpers;
using Application._Common.Security.Authentication;
using Application._Common.Settings;
using HealthChecks.UI.Client;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Presentation._Common.Endpoints;
using Presentation._Common.Middleware.ExceptionHandler;
using Presentation._Common.Security;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

namespace Presentation;

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
            .AddSecurityServices(configuration)
            .AddOpenApiServices()
            .AddSettingsServices(configuration);

        return builder;
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
    private static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<IIdentityService, IdentityService>()
            .AddScoped<ICurrentUserService, CurrentUserService>();

        var jwtSetting = configuration
            .GetRequiredSection(nameof(JwtSettings))
            .Get<JwtSettings>()
        ?? throw new NotFoundException($"Setting {nameof(JwtSettings)} was not found.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSetting.Issuer,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidAlgorithms = [SecurityAlgorithms.HmacSha512],
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

        services
            .AddAuthorizationBuilder()
            .AddDefaultPolicy("DefaultPolicy", policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
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
            .AddOptions<HashingSettings>()
            .Bind(configuration.GetRequiredSection(nameof(HashingSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetRequiredSection(nameof(JwtSettings)))
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

        return services;
    }
}

internal static partial class ConfigureBuilder
{
    public static WebApplication UsePresentation(this WebApplication app)
    {
        if (app.Environment.IsProduction())
            app.UseHsts();

        //IApplicationBuilder
        app
            .UseHttpsRedirection()
            .UseApplicationCors()
            .UseExceptionHandler()
            .UseSwaggerUserInterface()
            .UseSecurity();

        //IEndpointRouteBuilder
        app
            .UseHealthChecks()
            .UseEndpoints();

        return app;
    }

    private static IApplicationBuilder UseApplicationCors(this IApplicationBuilder app)
    {
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );

        return app;
    }
    private static IApplicationBuilder UseSwaggerUserInterface(this IApplicationBuilder app)
    {
        app
            .UseSwagger()
            .UseSwaggerUI();

        return app;
    }
    private static IApplicationBuilder UseSecurity(this IApplicationBuilder app)
    {
        app
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }
    private static IEndpointRouteBuilder UseHealthChecks(this IEndpointRouteBuilder app)
    {
        app
            .MapHealthChecks("/health", new()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        return app;
    }
    private static IEndpointRouteBuilder UseEndpoints(this IEndpointRouteBuilder app)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var endpointCollections = AbstractHelpers.CreateInstancesAssignableFromType<IEndpointCollection>(currentAssembly);

        foreach (var collection in endpointCollections)
            collection.RegisterEndpoints(app);

        return app;
    }
}
