using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Infrastructure.Persistence;
using ShipmentTracking.WebApi;
using ShipmentTracking.WebApi.Contracts.Auth;
using Testcontainers.PostgreSql;
using Xunit;

namespace ShipmentTracking.Integration.Tests.Infrastructure;

/// <summary>
/// Provides a configured <see cref="WebApplicationFactory{TEntryPoint}"/> backed by a PostgreSQL Testcontainer.
/// </summary>
public sealed class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string JwtSecret = "integration-tests-secret-key-should-be-long-1234567890";

    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithDatabase("shipment_tracking_it")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithImage("postgres:16-alpine")
            .WithCleanUp(true)
            .Build();

    private FakeEmailService? _fakeEmailService;

    /// <summary>
    /// Gets the seeded administrator email address used for authentication in tests.
    /// </summary>
    public string AdminEmail => "admin.integration@shipmenttracking.local";

    /// <summary>
    /// Gets the seeded administrator password used for authentication in tests.
    /// </summary>
    public string AdminPassword => "Admin#123456";

    /// <inheritdoc/>
    public async Task InitializeAsync() => await _postgresContainer.StartAsync();

    /// <inheritdoc/>
    public new async Task DisposeAsync() => await _postgresContainer.DisposeAsync();

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgresContainer.GetConnectionString(),
                ["Jwt:SecretKey"] = JwtSecret,
                ["Jwt:Issuer"] = "ShipmentTracking.IntegrationTests",
                ["Jwt:Audience"] = "ShipmentTracking.IntegrationTests",
            });

            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminUser:Email"] = AdminEmail,
                ["AdminUser:Password"] = AdminPassword,
                ["AdminUser:FirstName"] = "Integration",
                ["AdminUser:LastName"] = "Administrator",
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailService>();

            services.AddSingleton<FakeEmailService>();
            services.AddSingleton<IEmailService>(provider =>
            {
                _fakeEmailService = provider.GetRequiredService<FakeEmailService>();
                return _fakeEmailService;
            });

            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
                options.UseSnakeCaseNamingConvention();
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());
        });
    }

    /// <summary>
    /// Resolves the <see cref="FakeEmailService"/> instance registered for the test host.
    /// </summary>
    public FakeEmailService GetFakeEmailService()
    {
        if (_fakeEmailService is null)
        {
            using var scope = Services.CreateScope();
            _fakeEmailService = scope.ServiceProvider.GetRequiredService<FakeEmailService>();
        }

        return _fakeEmailService;
    }

    /// <summary>
    /// Creates an authenticated client representing the seeded administrator.
    /// </summary>
    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = AdminEmail,
            Password = AdminPassword
        });

        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new InvalidOperationException("Failed to deserialize login response.");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        return client;
    }
}
