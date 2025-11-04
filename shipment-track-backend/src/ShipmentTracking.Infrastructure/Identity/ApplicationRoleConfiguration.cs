using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShipmentTracking.Infrastructure.Identity;

/// <summary>
/// EF configuration for <see cref="ApplicationRole"/>.
/// </summary>
public sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("asp_net_roles");
        builder.Property(role => role.Name)
            .HasMaxLength(64);
    }
}
