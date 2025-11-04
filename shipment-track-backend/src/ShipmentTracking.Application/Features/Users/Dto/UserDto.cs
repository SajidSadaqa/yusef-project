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
}
