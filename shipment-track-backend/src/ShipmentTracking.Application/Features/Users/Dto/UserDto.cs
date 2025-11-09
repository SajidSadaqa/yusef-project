using System;
using System.Collections.Generic;

namespace ShipmentTracking.Application.Features.Users.Dto;

/// <summary>
/// Represents user details for administrative views.
/// </summary>
public sealed class UserDto
{
    public string Id { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? PhoneNumber { get; init; }

    public bool EmailConfirmed { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Primary role of the user (for frontend compatibility).
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Status of the user account.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// When the user account was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the user account was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}
