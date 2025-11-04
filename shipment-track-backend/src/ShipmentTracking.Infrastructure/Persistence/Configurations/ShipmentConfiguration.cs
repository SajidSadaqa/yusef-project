using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Domain.Enums;
using ShipmentTracking.Domain.ValueObjects;

namespace ShipmentTracking.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="Shipment"/>.
/// </summary>
public sealed class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("shipments");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrackingNumber)
            .HasConversion(
                tracking => tracking.Value,
                value => TrackingNumber.Create(value))
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(s => s.ReferenceNumber)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(s => s.CustomerReference)
            .HasMaxLength(128);

        builder.Property(s => s.OriginPort)
            .HasConversion(port => port.Code, value => Port.Create(value))
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(s => s.DestinationPort)
            .HasConversion(port => port.Code, value => Port.Create(value))
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(s => s.WeightKg)
            .HasConversion(weight => weight.Value, value => Weight.FromDecimal(value))
            .HasColumnType("numeric(18,3)")
            .IsRequired();

        builder.Property(s => s.VolumeCbm)
            .HasConversion(volume => volume.Value, value => Volume.FromDecimal(value))
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(s => s.CurrentStatus)
            .HasConversion(status => status.ToString(), value => Enum.Parse<ShipmentStatus>(value))
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(s => s.CurrentLocation)
            .HasMaxLength(256);

        builder.Property(s => s.Notes)
            .HasColumnType("text");

        builder.Property(s => s.CreatedByUserId);
        builder.Property(s => s.UpdatedByUserId);

        builder.HasIndex(s => s.TrackingNumber).IsUnique();
        builder.HasIndex(s => s.ReferenceNumber).IsUnique();
        builder.HasIndex(s => s.CurrentStatus);
        builder.HasIndex(s => new { s.EstimatedArrivalUtc, s.CurrentStatus });

        builder.Navigation(s => s.StatusHistory)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.StatusHistory)
            .WithOne(history => history.Shipment)
            .HasForeignKey(history => history.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
