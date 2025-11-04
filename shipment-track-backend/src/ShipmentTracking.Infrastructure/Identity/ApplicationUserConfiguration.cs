using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShipmentTracking.Infrastructure.Identity;

/// <summary>
/// EF configuration for <see cref="ApplicationUser"/>.
/// </summary>
public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("asp_net_users");

        builder.Property(user => user.FirstName)
            .HasMaxLength(64);

        builder.Property(user => user.LastName)
            .HasMaxLength(64);

        builder.Property(user => user.CreatedAtUtc)
            .IsRequired();

        builder.Property(user => user.LastLoginAtUtc);

        builder.Property(user => user.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(user => user.RefreshTokens)
            .WithOne()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
