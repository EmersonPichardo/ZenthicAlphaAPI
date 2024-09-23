using Application.Exceptions;
using FluentValidation;
using Identity.Application._Common.Authentication;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application._Common.Settings;
using Identity.Infrastructure._Common.Behaviors;
using Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure._Common.Persistence.Databases.Interceptors;
using Identity.Infrastructure._Common.Security;
using Infrastructure.Modularity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Identity.Infrastructure;

public partial class ConfigureBuilder : IInfrastructureInstaller
{
    public void AddInfrastructure(IServiceCollection services, IConfiguration configuration)
    {
        AddDbContextServices(services, configuration);
        AddSecurityServices(services, configuration);
        AddSettingsServices(services, configuration);
        AddFluentValidationServices(services);
        AddHealthChecksServices(services);
        AddMediatorServices(services);
    }

    private static void AddDbContextServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<IIdentityDbContext, IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(nameof(IdentityDbContext)),
                    builder => builder.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)
                )
            )
            .AddScoped<AuditableEntitySaveChangesInterceptor>();

    }
    private static void AddSecurityServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IPasswordHasher, PasswordHasher>()
            .AddTransient<IJwtService, JwtService>()
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
    }
    private static void AddSettingsServices(IServiceCollection services, IConfiguration configuration)
    {
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
    }
    private static void AddFluentValidationServices(IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            AssemblyReference.Assembly,
            includeInternalTypes: true
        );
    }
    private static void AddHealthChecksServices(IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<IdentityDbContext>();
    }
    private static void AddMediatorServices(IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration
                .RegisterServicesFromAssembly(AssemblyReference.Assembly)
                .AddOpenBehavior(typeof(AuthorizationBehavior<,>))
                .AddOpenBehavior(typeof(TransactionBehavior<,>));
        });
    }
}

public partial class ConfigureBuilder : IInfrastructureInstaller
{
    public void UseInfrastructure(IHost host)
    {
        ApplyMigrations(host);
    }

    private static void ApplyMigrations(IHost host)
    {
        var environment = host.Services
            .GetRequiredService<IHostEnvironment>();

        if (!environment.IsDevelopment())
            return;

        using var scope = host.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IIdentityDbContext>()
            .Database
            .MigrateAsync();
    }
}