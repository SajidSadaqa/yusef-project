using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="AuditLog"/>.
/// </summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Action)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(log => log.EntityName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(log => log.Details)
            .HasColumnType("text");

        builder.Property(log => log.OriginIp)
            .HasMaxLength(64);

        builder.Property(log => log.CorrelationId)
            .HasMaxLength(64);

        builder.Property(log => log.OccurredAtUtc)
            .IsRequired();

        builder.Property(log => log.UserId);

        builder.Property(log => log.EntityId);

        builder.HasIndex(log => log.OccurredAtUtc);
    }
}
