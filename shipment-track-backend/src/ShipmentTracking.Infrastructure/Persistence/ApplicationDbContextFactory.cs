using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Infrastructure.Persistence.Interceptors;
using ShipmentTracking.Infrastructure.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace ShipmentTracking.Infrastructure.Persistence;

/// <summary>
/// Design-time factory required for EF Core tooling.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=shipment_tracking;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        var interceptor = new AuditableEntitySaveChangesInterceptor(new DesignTimeCurrentUserService(), new UtcDateTimeProvider());

        return new ApplicationDbContext(optionsBuilder.Options, interceptor);
    }

    private sealed class DesignTimeCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => null;

        public string? Email => null;

        public IReadOnlyCollection<string> Roles => Array.Empty<string>();

        public bool IsAuthenticated => false;
    }
}
