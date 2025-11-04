using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Domain.Enums;

namespace ShipmentTracking.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="ShipmentStatusHistory"/>.
/// </summary>
public sealed class ShipmentStatusHistoryConfiguration : IEntityTypeConfiguration<ShipmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<ShipmentStatusHistory> builder)
    {
        builder.ToTable("shipment_status_histories");

        builder.HasKey(history => history.Id);

        builder.Property(history => history.Status)
            .HasConversion(status => status.ToString(), value => Enum.Parse<ShipmentStatus>(value))
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(history => history.Description)
            .HasMaxLength(512);

        builder.Property(history => history.Location)
            .HasMaxLength(256);

        builder.Property(history => history.AddedByUserId);

        builder.Property(history => history.EventTimeUtc)
            .IsRequired();

        builder.HasIndex(history => new { history.ShipmentId, history.EventTimeUtc });
    }
}
