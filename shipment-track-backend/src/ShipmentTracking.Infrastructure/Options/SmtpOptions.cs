using System.ComponentModel.DataAnnotations;

namespace ShipmentTracking.Infrastructure.Options;

/// <summary>
/// Options for configuring SMTP-based email delivery (e.g. MailHog).
/// </summary>
public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    [Required]
    [MaxLength(256)]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 25;

    public bool UseSsl { get; set; }

    [MaxLength(256)]
    public string? Username { get; set; }

    [MaxLength(256)]
    public string? Password { get; set; }

    [EmailAddress]
    public string? FromEmail { get; set; }

    [MaxLength(128)]
    public string? FromName { get; set; }
}
