using System.ComponentModel.DataAnnotations;

namespace ShipmentTracking.Infrastructure.Options;

/// <summary>
/// Options for configuring the Resend email provider.
/// </summary>
public sealed class ResendOptions
{
    public const string SectionName = "Resend";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = "Shipment Tracking";
}
