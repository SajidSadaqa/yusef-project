using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="PortMaster"/>.
/// </summary>
public sealed class PortMasterConfiguration : IEntityTypeConfiguration<PortMaster>
{
    public void Configure(EntityTypeBuilder<PortMaster> builder)
    {
        builder.ToTable("ports");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.City)
            .HasMaxLength(100);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedByUserId);
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedByUserId);
        builder.Property(p => p.UpdatedAtUtc);

        // Unique index on port code
        builder.HasIndex(p => p.Code)
            .IsUnique();

        // Index for active ports (frequently queried)
        builder.HasIndex(p => p.IsActive);

        // Index for searching by country
        builder.HasIndex(p => p.Country);
    }
}
