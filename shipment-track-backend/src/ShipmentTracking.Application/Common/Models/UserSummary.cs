using System;

namespace ShipmentTracking.Application.Common.Models;

/// <summary>
/// Lightweight representation of an identity user.
/// </summary>
public sealed class UserSummary
{
    /// <summary>
    /// Gets or sets the identifier of the user.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets or sets the user email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets or sets last name.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Gets a value indicating whether the email has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; init; }

    /// <summary>
    /// Gets or sets optional phone number.
    /// </summary>
    public string? PhoneNumber { get; init; }
}
