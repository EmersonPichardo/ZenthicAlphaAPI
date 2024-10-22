using Application.Auth;
using Application.Exceptions;
using Application.Persistence.Databases;
using FluentValidation;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.ModuleBehaviors;
using Identity.Infrastructure.Common.Settings;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Infrastructure.Behaviors;
using Infrastructure.Modularity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure;

public class Installer : IModuleInstaller
{
    public void AddInfrastructure(WebApplicationBuilder builder)
    {
        AddDbContextServices(builder);
        AddAuthServices(builder);
        AddFluentValidationServices(builder);
        AddHealthChecksServices(builder);
        AddMediatorServices(builder);
    }
    public void UseInfrastructure(WebApplication app)
    {
        ApplyMigrations(app);
    }

    private static void AddDbContextServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddDbContext<IApplicationDbContext, IdentityModuleDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(nameof(IdentityModuleDbContext)),
                    builder => builder.MigrationsAssembly(AssemblyReference.Assembly.FullName)
                )
            );
    }
    private static void AddAuthServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<AuthSettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(AuthSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddScoped<IUserSessionService, UserSessionSession>()
            .AddKeyedScoped<IModuleBehavior, AuthorizationModuleBehavior>(BehaviorsConstants.AuthorizationBehaviorName)
            .AddScoped<JwtManager>()
            .AddScoped<HashingManager>()
            .AddScoped<TokenManager>()
            .AddScoped<PasswordManager>();

        var authSettings = builder.Configuration
            .GetRequiredSection(nameof(AuthSettings))
            .Get<AuthSettings>()
        ?? throw new NotFoundException($"Setting {nameof(AuthSettings)} was not found.");

        var authBuilder = builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(EncodingHelper.GetBytes(authSettings.Jwt.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Environment.ApplicationName,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }
            );

        if (authSettings.OAuth?.Google is not null)
            authBuilder.AddGoogle(
                GoogleDefaults.AuthenticationScheme,
                GoogleDefaults.DisplayName,
                options =>
                {
                    options.ClientId = authSettings.OAuth!.Google!.ClientId;
                    options.ClientSecret = authSettings.OAuth!.Google!.ClientSecret;

                    options.ClaimActions.MapJsonKey(nameof(AuthenticatedSession.UserName), "name");
                    options.ClaimActions.MapJsonKey(nameof(AuthenticatedSession.Email), "email");

                    options.Events = new()
                    {
                        OnCreatingTicket = (OAuthCreatingTicketContext context) =>
                        {
                            context.Identity?.AddClaim(new(
                                nameof(AuthenticatedSession.Status), OAuthUserStatus.Active.ToString()
                            ));

                            return Task.CompletedTask;
                        }
                    };
                }
            );
    }
    private static void AddFluentValidationServices(WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(
            AssemblyReference.Assembly,
            includeInternalTypes: true
        );
    }
    private static void AddHealthChecksServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddHealthChecks()
            .AddDbContextCheck<IdentityModuleDbContext>();
    }
    private static void AddMediatorServices(WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(AssemblyReference.Assembly)
        );
    }

    private static void ApplyMigrations(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        using var scope = app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<IdentityModuleDbContext>();

        dbContext.Database.MigrateAsync();
    }
}
