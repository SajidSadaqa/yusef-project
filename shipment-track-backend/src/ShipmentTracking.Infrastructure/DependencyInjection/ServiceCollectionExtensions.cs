using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Infrastructure.Identity;
using ShipmentTracking.Infrastructure.Options;
using ShipmentTracking.Infrastructure.Persistence;
using ShipmentTracking.Infrastructure.Persistence.Interceptors;
using ShipmentTracking.Infrastructure.Seed;
using ShipmentTracking.Infrastructure.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace ShipmentTracking.Infrastructure.DependencyInjection;

/// <summary>
/// Service collection extensions for the infrastructure layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey), "JWT secret key must be provided.")
            .Validate(options => options.SecretKey.Length >= 32, "JWT secret key must be at least 32 characters.")
            .ValidateOnStart();

        services.Configure<ResendOptions>(configuration.GetSection(ResendOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.Configure<PortCatalogOptions>(configuration.GetSection(PortCatalogOptions.SectionName));

        services.AddOptions<AdminUserOptions>()
            .Bind(configuration.GetSection(AdminUserOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "Admin user password must be provided.")
            .ValidateOnStart();

        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
        services.AddSingleton<IPortCatalogService, PortCatalogService>();

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
            }

            options.UseNpgsql(connectionString);
            options.UseSnakeCaseNamingConvention();
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITrackingNumberGenerator, TrackingNumberGenerator>();
        services.AddScoped<IReferenceNumberGenerator, ReferenceNumberGenerator>();
        services.AddScoped<IPortCodeGenerator, PortCodeGenerator>();
        services.AddScoped<ITokenService, TokenService>();

        RegisterEmailService(services, configuration);

        services.AddScoped<IdentitySeeder>();
        services.AddScoped<ShipmentSeeder>();
        services.AddScoped<PortSeeder>();

        return services;
    }

    private static void RegisterEmailService(IServiceCollection services, IConfiguration configuration)
    {
        var resendApiKey = configuration.GetValue<string?>($"{ResendOptions.SectionName}:ApiKey");
        var smtpHost = configuration.GetValue<string?>($"{SmtpOptions.SectionName}:Host");

        if (!IsPlaceholder(resendApiKey))
        {
            services.AddHttpClient<EmailService>();
            services.AddScoped<IEmailService, EmailService>();
            return;
        }

        if (!string.IsNullOrWhiteSpace(smtpHost))
        {
            services.AddScoped<IEmailService, SmtpEmailService>();
            return;
        }

        throw new InvalidOperationException("Email provider configuration is missing. Configure Resend or Smtp settings.");
    }

    private static bool IsPlaceholder(string? apiKey) =>
        string.IsNullOrWhiteSpace(apiKey) ||
        apiKey.Equals("re_xxxxxxxxxxxx", StringComparison.OrdinalIgnoreCase);
}
