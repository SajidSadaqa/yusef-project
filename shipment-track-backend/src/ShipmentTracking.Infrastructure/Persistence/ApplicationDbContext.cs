using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Infrastructure.Identity;
using ShipmentTracking.Infrastructure.Persistence.Interceptors;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace ShipmentTracking.Infrastructure.Persistence;

/// <summary>
/// Application database context backed by PostgreSQL and ASP.NET Identity.
/// </summary>
public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntityInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntityInterceptor)
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();

    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<PortMaster> Ports => Set<PortMaster>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("uuid-ossp");
        builder.HasSequence<long>("tracking_number_seq")
            .StartsAt(1000)
            .IncrementsBy(1);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
