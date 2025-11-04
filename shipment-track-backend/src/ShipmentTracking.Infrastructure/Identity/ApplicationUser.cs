using Microsoft.AspNetCore.Identity;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Infrastructure.Identity;

/// <summary>
/// Custom identity user with additional profile data.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? LastLoginAtUtc { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
