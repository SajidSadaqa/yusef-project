using System.ComponentModel.DataAnnotations;

namespace ShipmentTracking.Infrastructure.Options;

/// <summary>
/// Configuration for seeding an initial administrator account.
/// </summary>
public sealed class AdminUserOptions
{
    public const string SectionName = "AdminUser";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "admin@shipmenttracking.local";

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = "System";

    public string LastName { get; set; } = "Administrator";
}
